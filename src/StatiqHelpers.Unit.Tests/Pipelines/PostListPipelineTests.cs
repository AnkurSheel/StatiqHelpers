using System.Linq;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Pipelines;
using VerifyXunit;
using Xunit;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    [UsesVerify]
    public class PostListPipelineTests : PipelineBaseFixture
    {
        private const string PipelineName = nameof(PostListPipeline);

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
        public async Task Sets_destination_to_blog()
        {
            var fileProvider = GetFileProvider();
            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int) ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();
            Assert.Equal("blog.html", document.Destination);
        }

        private TestFileProvider GetFileProvider()
        {
            var fileProvider = new TestFileProvider
            {
                "/input/Blog.cshtml",
            };
            return fileProvider;
        }
    }
}
