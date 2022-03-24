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
    public class ImagePipelineTests : BaseFixture
    {
        private const string PipelineName = nameof(ImagesPipeline);
        private readonly Bootstrapper _bootstrapper;

        public ImagePipelineTests()
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
        public async Task Verify_process_modules_cache()
        {
            await PipelineCommonTests.Verify_process_modules_cache(_bootstrapper, PipelineName);
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

        [Fact]
        public async Task Favicon_is_copied_to_the_root()
        {
            var path = "/input/assets/images/favicon.ico";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("favicon.ico", document.Destination);
        }

        [Fact]
        public async Task Post_image_in_the_post_folder_is_copied_to_the_correct_folder()
        {
            var path = "/input/posts/2021/2021-11-19-slug/cover.jpg";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/images/posts/slug/cover.jpg", document.Destination);
        }

        [Fact]
        public async Task Post_image_in_a_images_folder_under_the_post_folder_is_copied_to_the_correct_folder()
        {
            var path = "/input/posts/2021/2021-11-19-slug/images/cover.jpg";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/images/posts/slug/cover.jpg", document.Destination);
        }

        [Fact]
        public async Task Page_image_in_the_post_folder_is_copied_to_the_correct_folder()
        {
            var path = "/input/pages/slug/cover.jpg";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/images/pages/slug/cover.jpg", document.Destination);
        }

        [Fact]
        public async Task Page_image_in_a_images_folder_under_the_page_folder_is_copied_to_the_correct_folder()
        {
            var path = "/input/pages/slug/images/cover.jpg";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/images/pages/slug/cover.jpg", document.Destination);
        }

        [Fact]
        public async Task All_images_are_copied_to_the_correct_folder()
        {
            var path = "/input/assets/images/cover.jpg";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/images/cover.jpg", document.Destination);
        }
    }
}
