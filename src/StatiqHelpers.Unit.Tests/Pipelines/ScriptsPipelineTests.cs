using Statiq.Testing;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    [UsesVerify]
    public class ScriptsPipelineTests : PipelineBaseFixture
    {
        protected override string PipelineName => nameof(ScriptsPipeline);

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
        public async Task Js_files_are_copied_to_the_js_folder()
        {
            var path = "/input/assets/js/scripts.js";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("assets/js/scripts.js", document.Destination.ToString());
        }

        [Fact]
        public async Task Sw_Js_files_is_copied_to_the_root()
        {
            var path = "/input/assets/js/sw.js";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal("sw.js", document.Destination.ToString());
        }
    }
}