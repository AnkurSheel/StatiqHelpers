using System.Xml;
using Statiq.App;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Unit.Tests.Pipelines;

[UsesVerify]
public class SitemapPipelineTests : PipelineBaseFixture
{
    private const string PipelineName = nameof(SitemapPipeline);

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
    public async Task Sitemap_is_created_for_all_pages_and_posts()
    {
        var fileProvider = GetFileProvider();
        var result = await Bootstrapper.RunTestAsync(fileProvider);

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);
        var document = result.Outputs[PipelineName][Phase.Output].Single();
        var content = await document.GetContentStringAsync();

        var doc = new XmlDocument();
        doc.LoadXml(content);
        await Verify(doc);
    }

    [Fact]
    public async Task Sets_destination_to_sitemap_xml()
    {
        var fileProvider = GetFileProvider();
        var result = await Bootstrapper.RunTestAsync(fileProvider);
        var document = result.Outputs[PipelineName][Phase.Output].Single();

        Assert.Equal((int)ExitCode.Normal, result.ExitCode);

        Assert.Equal("sitemap.xml", document.Destination.ToString());
    }

    private TestFileProvider GetFileProvider()
    {
        var fileProvider = new TestFileProvider
        {
            "/input/Index.cshtml",
            "/input/pages/markdownSlug/markdownFile.md",
            "/input/pages/razorSlug/razorFile.cshtml",
            "/input/posts/2022-03-27-slug/filename.md",
        };
        return fileProvider;
    }
}
