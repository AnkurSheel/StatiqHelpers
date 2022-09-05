using Statiq.Testing;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Unit.Tests.Pipelines;

[UsesVerify]
public class TagsListPipelineTests : PipelineBaseFixture
{
    private const string PipelineName = nameof(TagsListPipeline);

    [Fact]
    public async Task Verify_dependencies()
    {
        await VerifyDependencies(PipelineName);
    }

    [Fact]
    public async Task Verify_input_modules()
    {
        await VerifyInputModules(PipelineName);
    }

    [Fact]
    public async Task Verify_process_modules()
    {
        await VerifyProcessModules(PipelineName);
    }

    [Fact]
    public async Task Verify_post_process_modules()
    {
        await VerifyPostProcessModules(PipelineName);
    }

    [Fact]
    public async Task Verify_output_modules()
    {
        await VerifyOutputModules(PipelineName);
    }

    [Fact]
    public async Task Sets_destination_to_tags()
    {
        var fileProvider = GetFileProvider();
        var result = await Bootstrapper.RunTestAsync(fileProvider);

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);
        var document = result.Outputs[PipelineName][Phase.Output].Single();
        Assert.Equal("tags.html", document.Destination.ToString());
    }

    [Fact]
    public async Task Title_is_set_to_All_Tags()
    {
        var fileProvider = GetFileProvider();
        var result = await Bootstrapper.RunTestAsync(fileProvider);

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);
        var document = result.Outputs[PipelineName][Phase.Output].Single();
        Assert.Equal("All Tags", document[Keys.Title]);
    }

    private TestFileProvider GetFileProvider()
    {
        var fileProvider = new TestFileProvider
        {
            "/input/TagsList.cshtml",
        };
        return fileProvider;
    }
}
