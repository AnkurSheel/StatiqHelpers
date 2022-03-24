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
    public class RssPipelineTests : BaseFixture
    {
        private readonly Bootstrapper _bootstrapper;
        private const string PipelineName = nameof(RssPipeline);

        public RssPipelineTests()
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

        [Fact]
        public async Task Sets_destination_correctly()
        {
            var result = await _bootstrapper.RunTestAsync();

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();
            Assert.Equal("rss.xml", document.Destination);
        }
    }
}
