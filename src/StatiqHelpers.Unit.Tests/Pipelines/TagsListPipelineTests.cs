using Statiq.Testing;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Unit.Tests.Pipelines;

[UsesVerify]
public class TagsListPipelineTests : PipelineBaseFixture
{
    protected override string PipelineName => nameof(TagsListPipeline);
    protected override string[] DependentPipelineNames => new[] { nameof(TagsPipeline), nameof(PostPipeline) };

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
            "/input/TagsList.cshtml"
        };
        return fileProvider;
    }
}