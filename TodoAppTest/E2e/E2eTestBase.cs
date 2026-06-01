using Microsoft.Extensions.DependencyInjection;
using TodoAppApi;
using TodoAppTest.E2e.Uitls;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e;

[Category("E2e")]
[NonParallelizable]
public abstract class E2ETestBase
{
    [SetUp]
    public void SetUp()
    {
        if (TestContext.CurrentContext.Test.Properties.Get("Seeder") is not string seeder)
            return;

        var db = E2ETestRuntime.ApiFactory.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var seederInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;
        seederInstance.Clear(db);
        seederInstance.Seed(db);
    }

    [TearDown]
    public void TearDown()
    {
        var db = E2ETestRuntime.ApiFactory.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (TestContext.CurrentContext.Test.Properties.Get("Seeder") is string seeder)
        {
            var seederInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;
            seederInstance.Clear(db);
        }
        else
            new DefaultSeeder().Clear(db);
    }

}
