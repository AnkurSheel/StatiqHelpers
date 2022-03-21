using System.Linq;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Pipelines;
using Xunit;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    public class CssPipelineTests : BaseFixture
    {
        private readonly Bootstrapper _bootstrapper;

        public CssPipelineTests()
        {
            BaseSetUp();
            _bootstrapper = PipelineTestHelpersStatic.GetBootstrapper();
        }

        [Fact]
        public async Task Css_files_are_copied_to_the_root()
        {
            var path = "/input/assets/styles.css";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[nameof(CssPipeline)][Phase.Output].Single();

            Assert.Equal("assets/styles.css", document.Destination);
        }

        [Fact]
        public async Task Css_files_staring_with_underscore_are_not_copied_to_the_root()
        {
            var path = "/input/assets/images/_site.css";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var documents = result.Outputs[nameof(CssPipeline)][Phase.Output];

            Assert.Empty(documents);
        }
    }
}
