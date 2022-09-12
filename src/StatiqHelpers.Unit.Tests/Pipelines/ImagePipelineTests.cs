using Statiq.Testing;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    [UsesVerify]
    public class ImagePipelineTests : PipelineBaseFixture
    {
        private const string PipelineName = nameof(ImagesPipeline);

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
        public async Task Favicon_is_copied_to_the_root()
        {
            var path = "/input/assets/images/favicon.ico";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("favicon.ico", document.Destination.ToString());
        }

        [Fact]
        public async Task Post_image_in_the_post_folder_is_copied_to_the_correct_folder()
        {
            var path = "/input/posts/2021/2021-11-19-slug/cover.jpg";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/images/posts/slug/cover.jpg", document.Destination.ToString());
        }

        [Fact]
        public async Task Post_image_in_the_post_folder_is_copied_to_a_folder_without_filler_words()
        {
            var path = "/input/posts/2021/2021-11-19-to-slug-from/cover.jpg";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/images/posts/slug-/cover.jpg", document.Destination.ToString());
        }

        [Fact]
        public async Task Post_image_in_a_images_folder_under_the_post_folder_is_copied_to_the_correct_folder()
        {
            var path = "/input/posts/2021/2021-11-19-slug/images/cover.jpg";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/images/posts/slug/cover.jpg", document.Destination.ToString());
        }

        [Fact]
        public async Task Page_image_in_the_post_folder_is_copied_to_the_correct_folder()
        {
            var path = "/input/pages/slug/cover.jpg";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/images/pages/slug/cover.jpg", document.Destination.ToString());
        }

        [Fact]
        public async Task Page_image_in_a_images_folder_under_the_page_folder_is_copied_to_the_correct_folder()
        {
            var path = "/input/pages/slug/images/cover.jpg";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/images/pages/slug/cover.jpg", document.Destination.ToString());
        }

        [Fact]
        public async Task All_images_are_copied_to_the_correct_folder()
        {
            var path = "/input/assets/images/cover.jpg";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/images/cover.jpg", document.Destination.ToString());
        }
    }
}
