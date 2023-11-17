using Statiq.Testing;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    [UsesVerify]
    public class FontsPipelineTests : PipelineBaseFixture
    {
        protected override string PipelineName => nameof(FontsPipeline);

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

        [Theory]
        [InlineData("ttf")]
        [InlineData("woff2")]
        public async Task Binary_files_are_copied_to_the_fonts_folder(string extension)
        {
            var path = $"/input/assets/folder/filename.{extension}";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal($"assets/fonts/filename.{extension}", document.Destination.ToString());
        }
    }
}