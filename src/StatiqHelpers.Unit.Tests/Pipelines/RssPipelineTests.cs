using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Statiq.Testing;
using StatiqHelpers.Modules.RelatedPosts;
using StatiqHelpers.Pipelines;
using Keys = Statiq.Common.Keys;

namespace StatiqHelpers.Unit.Tests.Pipelines;

[UsesVerify]
public class RssPipelineTests : PipelineBaseFixture
{
    public RssPipelineTests()
    {
        Bootstrapper.ConfigureServices(services => services.AddSingleton(Mock.Of<IRelatedPostsService>()))
            .AddSetting(Keys.Host, "statiqhelpers.com");
    }

    protected override string PipelineName => nameof(RssPipeline);
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
    public async Task Sets_destination_correctly()
    {
        var result = await Bootstrapper.RunTestAsync();

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);
        var document = result.Outputs[PipelineName][Phase.Output].Single();
        Assert.Equal("rss.xml", document.Destination.ToString());
    }

    [Fact]
    public async Task Rss_feed_is_created_for_posts()
    {
        var settings = new VerifyTests.VerifySettings();
        var counter = 0;
        settings.ScrubLinesWithReplace(line =>
        {
            if (counter == 0 && line.Contains("<pubDate>"))
            {
                counter++;
                return "<pubDate>pubdate</pubDate>";
            }

            return line;
        });

        settings.ScrubLinesWithReplace(line =>
            line.Contains("<lastBuildDate>") ? "<lastBuildDate>lastBuildDate</lastBuildDate>" : line);

        settings.ScrubLinesWithReplace(line => line.Contains("<copyright>") ? "<copyright>year</copyright>" : line);

        var fileProvider = GetFileProvider();
        var result = await Bootstrapper.RunTestAsync(fileProvider);

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);
        var document = result.Outputs[PipelineName][Phase.Output].Single();
        var content = await document.GetContentStringAsync();

        var doc = new XmlDocument();
        doc.LoadXml(content);
        await Verify(doc, settings);
    }

    private TestFileProvider GetFileProvider()
    {
        var fileProvider = new TestFileProvider
        {
            "/input/Index.cshtml",
            "/input/pages/markdownSlug/markdownFile.md",
            "/input/posts/2022-02-27-slug1/filename.md",
            "/input/posts/2022-03-27-slug2/filename.md"
        };
        return fileProvider;
    }
}
