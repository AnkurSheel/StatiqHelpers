using Statiq.App;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    [UsesVerify]
    public class ScriptsPipelineTests : PipelineBaseFixture
    {
        private const string PipelineName = nameof(ScriptsPipeline);

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
