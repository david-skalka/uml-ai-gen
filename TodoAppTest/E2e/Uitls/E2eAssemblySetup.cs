namespace TodoAppTest.E2e.Uitls;

[SetUpFixture]
public class E2EAssemblySetup
{
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        E2ETestRuntime.DisposeCurrent();
    }
}