using Statiq.Testing;

namespace StatiqHelpers.Unit.Tests.Pipelines;

[UsesVerify]
public class PipelineTests : BaseFixture
{
    private readonly Bootstrapper _bootstrapper;

    public PipelineTests()
    {
        _bootstrapper = PipelineTestHelpersStatic.GetBootstrapper().AddSetting(Keys.Host, "statiqhelpers.com");
        BaseSetUp();
    }

    [Fact]
    public async Task VerifyPipelines()
    {
        var result = await _bootstrapper.RunTestAsync(new TestFileProvider());

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);

        var pipelineNames = result.Engine.Pipelines.Keys.ToList();
        pipelineNames.Sort();
        await Verify(pipelineNames);
    }
}
