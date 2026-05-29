using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TodoAppApi;
using TodoAppTest.Integration.Infrastructure;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.Integration;

[NonParallelizable]
public abstract class IntegrationApiTests
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory<Program> Factory = new();

    protected IntegrationApiTests()
    {
        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [SetUp]
    public void MockSetup()
    {
        if (TestContext.CurrentContext.Test.Properties.Get("Seeder") is not string seeder)
            return;

        var seederInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;
        seederInstance.Seed(
            Factory.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>());
    }

    [TearDown]
    public void MockTeardown()
    {
        var db = Factory.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (TestContext.CurrentContext.Test.Properties.Get("Seeder") is string seeder)
        {
            var seederInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;
            seederInstance.Clear(db);
            return;
        }

        new DefaultSeeder().Clear(db);
    }

    [OneTimeTearDown]
    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
    }
}
