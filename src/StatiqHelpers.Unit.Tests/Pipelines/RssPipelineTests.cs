using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Statiq.App;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Modules.RelatedPosts;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    [UsesVerify]
    public class RssPipelineTests : PipelineBaseFixture
    {
        private const string PipelineName = nameof(RssPipeline);

        public RssPipelineTests()
        {
            Bootstrapper.ConfigureServices(services => services.AddSingleton(Mock.Of<IRelatedPostsService>()));
        }

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
            settings.ScrubLinesWithReplace(
                line =>
                {
                    if (counter == 0 && line.Contains("<pubDate>"))
                    {
                        counter++;
                        return "<pubDate>pubdate</pubDate>";
                    }

                    return line;
                });

            settings.ScrubLinesWithReplace(
                line => line.Contains("<lastBuildDate>")
                    ? "<lastBuildDate>lastBuildDate</lastBuildDate>"
                    : line);

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
                "/input/posts/2022-03-27-slug2/filename.md",
            };
            return fileProvider;
        }
    }
}
