using System.Linq;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Pipelines;
using Xunit;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    public class DownloadsPipelineTests : BaseFixture
    {
        private readonly Bootstrapper _bootstrapper;
        private const string PipelineName = nameof(DownloadsPipeline);

        public DownloadsPipelineTests()
        {
            BaseSetUp();
            _bootstrapper = PipelineTestHelpersStatic.GetBootstrapper();
        }

        [Theory]
        [InlineData("pdf")]
        [InlineData("zip")]
        [InlineData("rar")]
        [InlineData("exe")]
        public async Task Binary_files_are_copied_to_the_downloads_folder(string extension)
        {
            var path = $"/input/assets/folder/filename.{extension}";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();

            Assert.Equal($"assets/downloads/filename.{extension}", document.Destination);
        }
    }
}
