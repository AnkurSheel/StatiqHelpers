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
    public class CssPipelineTests : PipelineBaseFixture
    {
        private const string PipelineName = nameof(CssPipeline);

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
        public async Task Css_files_are_copied_to_the_root()
        {
            var path = "/input/assets/styles.css";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int) ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/styles.css", document.Destination);
        }

        [Fact]
        public async Task Css_files_staring_with_underscore_are_not_copied_to_the_root()
        {
            var path = "/input/assets/images/_site.css";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int) ExitCode.Normal, result.ExitCode);
            var documents = result.Outputs[PipelineName][Phase.Output];

            Assert.Empty(documents);
        }
    }
}
