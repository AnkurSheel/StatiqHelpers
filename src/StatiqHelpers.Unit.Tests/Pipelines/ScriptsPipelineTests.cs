using System.Linq;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Pipelines;
using Xunit;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    public class ScriptsPipelineTests : BaseFixture
    {
        private readonly Bootstrapper _bootstrapper;
        private const string PipelineName = nameof(ScriptsPipeline);

        public ScriptsPipelineTests()
        {
            BaseSetUp();
            _bootstrapper = PipelineTestHelpersStatic.GetBootstrapper();
        }

        [Fact]
        public async Task Js_files_are_copied_to_the_js_folder()
        {
            var path = "/input/assets/js/scripts.js";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal($"assets/js/scripts.js", document.Destination);
        }

        [Fact]
        public async Task Sw_Js_files_is_copied_to_the_root()
        {
            var path = "/input/assets/js/sw.js";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal($"sw.js", document.Destination);
        }
    }
}
