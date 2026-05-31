using TodoAppTest.E2e.Uitls;

namespace TodoAppTest.E2e;

[SetUpFixture]
public class E2eAssemblySetup
{
    [OneTimeTearDown]
    public void OneTimeTearDown() =>
        E2eTestRuntime.DisposeCurrent();
}
