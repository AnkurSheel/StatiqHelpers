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
    public class FontsPipelineTests : PipelineBaseFixture
    {
        private const string PipelineName = nameof(FontsPipeline);

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

        [Theory]
        [InlineData("ttf")]
        [InlineData("woff2")]
        public async Task Binary_files_are_copied_to_the_fonts_folder(string extension)
        {
            var path = $"/input/assets/folder/filename.{extension}";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int) ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal($"assets/fonts/filename.{extension}", document.Destination);
        }
    }
}
