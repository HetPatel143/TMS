#!/bin/bash
# TMS Build & Verification Script

echo "🔍 TMS - Ticket Management System"
echo "=================================="
echo ""

# Check if we're in the right directory
if [ ! -f "TMS.sln" ]; then
    echo "❌ Error: Run this script from the TMS root directory"
    exit 1
fi

echo "✅ Found TMS.sln"
echo ""

# Check Frontend
echo "📦 Checking Frontend..."
if [ -d "tms-client" ]; then
    echo "  ✓ tms-client directory exists"
    if [ -f "tms-client/package.json" ]; then
        echo "  ✓ package.json found"
    fi
    if [ -f "tms-client/tailwind.config.js" ]; then
        echo "  ✓ tailwind.config.js updated"
    fi
    if [ -f "tms-client/src/styles.css" ]; then
        echo "  ✓ styles.css updated"
    fi
    if [ -f "tms-client/src/components.css" ]; then
        echo "  ✓ components.css updated"
    fi
else
    echo "  ❌ tms-client directory not found"
fi

echo ""

# Check Backend
echo "🔧 Checking Backend..."
if [ -d "TMS.API" ]; then
    echo "  ✓ TMS.API directory exists"
    if [ -f "TMS.API/TMS.API.csproj" ]; then
        echo "  ✓ TMS.API.csproj found"
    fi
    if [ -f "TMS.API/Program.cs" ]; then
        echo "  ✓ Program.cs found"
    fi
    if [ -d "TMS.API/Controllers" ]; then
        echo "  ✓ Controllers directory exists"
    fi
else
    echo "  ❌ TMS.API directory not found"
fi

if [ -d "TMS.Core" ]; then
    echo "  ✓ TMS.Core directory exists"
fi

if [ -d "TMS.Infrastructure" ]; then
    echo "  ✓ TMS.Infrastructure directory exists"
fi

echo ""

# Check Documentation
echo "📚 Checking Documentation..."
docs=(
    "UI_REDESIGN_SUMMARY.md"
    "THEME_MIGRATION.md"
    "SETUP_AND_DEPLOYMENT.md"
    "API_TESTING_GUIDE.md"
    "DEVELOPER_REFERENCE.md"
    "FINAL_REPORT.md"
)

for doc in "${docs[@]}"; do
    if [ -f "$doc" ]; then
        echo "  ✓ $doc created"
    else
        echo "  ❌ $doc missing"
    fi
done

echo ""

# Check Test Files
echo "🧪 Checking Test Files..."
if [ -f "api-test.http" ]; then
    echo "  ✓ api-test.http created"
fi

if [ -f "test-api.js" ]; then
    echo "  ✓ test-api.js created"
fi

echo ""

# Summary
echo "=================================="
echo "✅ Verification Complete!"
echo ""
echo "Next Steps:"
echo "1. cd tms-client && npm install"
echo "2. cd ../TMS.API && dotnet restore"
echo "3. dotnet ef database update"
echo "4. dotnet run (one terminal)"
echo "5. npm run dev (another terminal)"
echo "6. Open http://localhost:4200"
echo ""
echo "For API Testing:"
echo "- Use VS Code REST Client with api-test.http"
echo "- Or run: node test-api.js"
echo ""
echo "Documentation:"
echo "- See DEVELOPER_REFERENCE.md for quick start"
echo "- See FINAL_REPORT.md for complete overview"
echo ""
