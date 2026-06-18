using BCrypt.Net;

// Generate BCrypt hashes for seed users
var passwords = new[] { "Admin@123", "Password@123" };
foreach (var pwd in passwords)
{
    var hash = BCrypt.Net.BCrypt.HashPassword(pwd);
    Console.WriteLine($"{pwd} => {hash}");
}
