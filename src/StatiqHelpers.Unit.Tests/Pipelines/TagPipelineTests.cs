using Statiq.App;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Unit.Tests.Pipelines;

[UsesVerify]
public class TagPipelineTests : PipelineBaseFixture
{
    private readonly string _content;

    public TagPipelineTests()
    {
        _content = @"
tags:
    - Tag 1
---
";
    }

    private const string PipelineName = nameof(TagsPipeline);

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
    public async Task Title_is_generated_based_on_the_tag()
    {
        var fileProvider = GetFileProvider();

        var result = await Bootstrapper.RunTestAsync(fileProvider);

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);
        var document = result.Outputs[PipelineName][Phase.Output].Single();

        Assert.Equal("Posts tagged as \"Tag 1\"", document.GetTitle());
    }

    [Fact]
    public async Task Tag_name_is_set_correctly()
    {
        var fileProvider = GetFileProvider();
        var result = await Bootstrapper.RunTestAsync(fileProvider);

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);
        var document = result.Outputs[PipelineName][Phase.Output].Single();
        Assert.Equal("Tag 1", document[MetaDataKeys.TagName]);
    }

    [Fact]
    public async Task Destination_is_set_correctly()
    {
        const string content = @"
tags:
    - Tag and   1
---
    ";

        var fileProvider = GetFileProvider(content);
        var result = await Bootstrapper.RunTestAsync(fileProvider);

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);
        var document = result.Outputs[PipelineName][Phase.Output].Single();
        Assert.Equal("tags/tag-1.html", document.Destination.ToString());
    }

    [Fact]
    public async Task Posts_are_grouped_correctly()
    {
        var fileProvider = new TestFileProvider
        {
            "/input/Tags.cshtml",
            { "/input/posts/2021/2021-11-19-slug2/slug1.md", _content },
            { "/input/posts/2021/2021-11-19-slug1/slug2.md", _content },
        };
        var result = await Bootstrapper.RunTestAsync(fileProvider);

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);
        var document = result.Outputs[PipelineName][Phase.Output].Single();
        Assert.Equal(2, document.GetChildren().Count);
    }

    [Fact]
    public async Task A_page_is_generated_for_each_tag()
    {
        const string content = @"
tags:
    - Tag 1
    - Tag 2
---
";
        var fileProvider = GetFileProvider(content);

        var result = await Bootstrapper.RunTestAsync(fileProvider);

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);
        var documents = result.Outputs[PipelineName][Phase.Output];

        Assert.Equal(2, documents.Length);
    }

    private TestFileProvider GetFileProvider(string? content = null)
    {
        content ??= _content;

        var fileProvider = new TestFileProvider
        {
            "/input/Tags.cshtml",
            { "/input/posts/2021/2021-11-19-slug/slug.md", content },
        };
        return fileProvider;
    }
}
