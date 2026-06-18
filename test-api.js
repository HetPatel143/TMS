#!/usr/bin/env node

const http = require('http');
const https = require('https');
const url = require('url');

const BASE_URL = 'http://localhost:5000/api';
let authToken = '';

const demoAccounts = {
  admin: { email: 'admin@company.com', password: 'Admin@123' },
  employee: { email: 'john.doe@company.com', password: 'Employee@123' },
  manager: { email: 'jane.smith@company.com', password: 'Manager@123' },
};

async function makeRequest(method, path, body = null, token = null) {
  return new Promise((resolve, reject) => {
    const urlObj = new URL(path, BASE_URL);
    const options = {
      hostname: urlObj.hostname,
      port: urlObj.port || 80,
      path: urlObj.pathname + urlObj.search,
      method: method,
      headers: {
        'Content-Type': 'application/json',
      },
    };

    if (token) {
      options.headers['Authorization'] = `Bearer ${token}`;
    }

    const req = http.request(options, (res) => {
      let data = '';
      res.on('data', (chunk) => {
        data += chunk;
      });
      res.on('end', () => {
        try {
          const parsed = JSON.parse(data);
          resolve({ status: res.statusCode, data: parsed, headers: res.headers });
        } catch {
          resolve({ status: res.statusCode, data: data, headers: res.headers });
        }
      });
    });

    req.on('error', reject);

    if (body) {
      req.write(JSON.stringify(body));
    }
    req.end();
  });
}

async function test(description, method, path, body = null, token = null, expectedStatus = 200) {
  try {
    const result = await makeRequest(method, path, body, token);
    const passed = result.status === expectedStatus;
    const symbol = passed ? '✓' : '✗';
    console.log(`${symbol} ${description}`);
    console.log(`  Status: ${result.status} (expected ${expectedStatus})`);
    if (!passed && result.data) {
      console.log(`  Response:`, result.data);
    }
    return result;
  } catch (error) {
    console.log(`✗ ${description}`);
    console.log(`  Error: ${error.message}`);
    return null;
  }
}

async function runTests() {
  console.log('\n🧪 TMS API Testing Suite\n');
  console.log('='.repeat(50));

  // 1. Test Authentication
  console.log('\n📝 Authentication Tests');
  console.log('-'.repeat(50));
  
  const loginResult = await test(
    'Login - Admin Account',
    'POST',
    '/auth/login',
    demoAccounts.admin,
    null,
    200
  );

  if (loginResult && loginResult.data && loginResult.data.data && loginResult.data.data.token) {
    authToken = loginResult.data.data.token;
    console.log('✓ Token obtained for subsequent requests');
  } else {
    console.log('✗ Failed to obtain auth token');
    return;
  }

  // 2. Test Dashboard Endpoints
  console.log('\n📊 Dashboard Tests');
  console.log('-'.repeat(50));
  
  await test('Get Employee Dashboard', 'GET', '/dashboard/employee', null, authToken, 200);
  await test('Get Approver Dashboard', 'GET', '/dashboard/approver', null, authToken, 200);
  await test('Get Admin Dashboard', 'GET', '/dashboard/admin', null, authToken, 200);

  // 3. Test Request Endpoints
  console.log('\n📋 Request Tests');
  console.log('-'.repeat(50));
  
  await test('Get All Requests', 'GET', '/requests?pageNumber=1&pageSize=10', null, authToken, 200);
  await test('Get Request #1', 'GET', '/requests/1', null, authToken, null); // May fail if request doesn't exist

  // 4. Test Request Types
  console.log('\n🏷️  Request Type Tests');
  console.log('-'.repeat(50));
  
  await test('Get All Request Types', 'GET', '/admin/request-types', null, authToken, 200);

  // 5. Test User Endpoints
  console.log('\n👥 User Tests');
  console.log('-'.repeat(50));
  
  await test('Get All Users', 'GET', '/admin/users', null, authToken, 200);

  // 6. Test Approval Endpoints
  console.log('\n✅ Approval Tests');
  console.log('-'.repeat(50));
  
  await test('Get Pending Approvals', 'GET', '/approvals/pending', null, authToken, 200);

  // 7. Test Reports
  console.log('\n📈 Report Tests');
  console.log('-'.repeat(50));
  
  await test(
    'Get Reports',
    'GET',
    '/reports?fromDate=2024-01-01&toDate=2024-12-31&status=All',
    null,
    authToken,
    200
  );

  console.log('\n' + '='.repeat(50));
  console.log('✨ Testing complete!\n');
}

// Run the tests
runTests().catch((error) => {
  console.error('Fatal error:', error);
  process.exit(1);
});
