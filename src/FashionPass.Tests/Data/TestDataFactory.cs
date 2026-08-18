using FashionPass.Tests.Config;
using FashionPass.Tests.Data.Models;

namespace FashionPass.Tests.Data;

public static class TestDataFactory
{
    public static User CreateRandomUser()
    {
        var stamp = Guid.NewGuid().ToString("N")[..8];
        return new User
        {
            Email = $"qa.{stamp}@fashionpass.test",
            Password = $"Passw0rd!{stamp}",
            FirstName = $"QA{stamp[..4]}",
            LastName = "Tester",
            Phone = $"555{new Random().Next(1000000, 9999999)}"
        };
    }
}