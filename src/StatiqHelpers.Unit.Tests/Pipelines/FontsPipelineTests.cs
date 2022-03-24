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
    public class FontsPipelineTests : BaseFixture
    {
        private const string PipelineName = nameof(FontsPipeline);
        private readonly Bootstrapper _bootstrapper;

        public FontsPipelineTests()
        {
            BaseSetUp();
            _bootstrapper = PipelineTestHelpersStatic.GetBootstrapper();
        }

        [Fact]
        public async Task Verify_dependencies()
        {
            await PipelineCommonTests.Verify_dependencies(_bootstrapper, PipelineName);
        }

        [Fact]
        public async Task Verify_input_modules()
        {
            await PipelineCommonTests.Verify_input_modules(_bootstrapper, PipelineName);
        }

        [Fact]
        public async Task Verify_process_modules()
        {
            await PipelineCommonTests.Verify_process_modules(_bootstrapper, PipelineName);
        }

        [Fact]
        public async Task Verify_post_process_modules()
        {
            await PipelineCommonTests.Verify_post_process_modules(_bootstrapper, PipelineName);
        }

        [Fact]
        public async Task Verify_output_modules()
        {
            await PipelineCommonTests.Verify_output_modules(_bootstrapper, PipelineName);
        }

        [Theory]
        [InlineData("ttf")]
        [InlineData("woff2")]
        public async Task Binary_files_are_copied_to_the_fonts_folder(string extension)
        {
            var path = $"/input/assets/folder/filename.{extension}";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal($"assets/fonts/filename.{extension}", document.Destination);
        }
    }
}
