using Statiq.Testing;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Unit.Tests.Pipelines;

[UsesVerify]
public class HomePipelineTests : PipelineBaseFixture
{
    protected override string PipelineName => nameof(HomePipeline);
    protected override string[] DependentPipelineNames => new[] { nameof(PostPipeline) };

    [Fact]
    public async Task Verify_dependencies()
    {
        await VerifyDependencies();
    }

    [Fact]
    public async Task Verify_input_modules()
    {
        await VerifyInputModules();
    }

    [Fact]
    public async Task Verify_process_modules()
    {
        await VerifyProcessModules();
    }

    [Fact]
    public async Task Verify_post_process_modules()
    {
        await VerifyPostProcessModules();
    }

    [Fact]
    public async Task Verify_output_modules()
    {
        await VerifyOutputModules();
    }

    [Fact]
    public async Task Sets_destination_to_index()
    {
        var fileProvider = GetFileProvider();
        var result = await Bootstrapper.RunTestAsync(fileProvider);

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);
        var document = result.Outputs[PipelineName][Phase.Output].Single();
        Assert.Equal("index.html", document.Destination.ToString());
    }

    private TestFileProvider GetFileProvider()
    {
        var fileProvider = new TestFileProvider
        {
            "/input/Index.cshtml",
            "/input/OtherFile.cshtml"
        };
        return fileProvider;
    }
}