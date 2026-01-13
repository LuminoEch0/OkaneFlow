using Service;

// See https://aka.ms/new-console-template for more information
var something = new Service.PasswordManager();
string password = something.HashPassword("admin");
Console.WriteLine($"The password is '{password}'");
