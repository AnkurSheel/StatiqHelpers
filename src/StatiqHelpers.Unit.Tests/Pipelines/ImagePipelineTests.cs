using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Statiq.App;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Extensions;
using StatiqHelpers.ImageHelpers;
using StatiqHelpers.Pipelines;
using Xunit;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    public class ImagePipelineTests : BaseFixture
    {
        private readonly Bootstrapper _bootstrapper;

        public ImagePipelineTests()
        {
            BaseSetUp();


            _bootstrapper = Bootstrapper.Factory.InitStatiq(Array.Empty<string>());
            _bootstrapper.ConfigureAnalyzers(
                collection =>
                {
                    foreach (var (key, value) in collection)
                    {
                        value.LogLevel = LogLevel.None;
                    }
                });
            _bootstrapper.ConfigureServices(services => services.AddSingleton(Mock.Of<IImageService>()));
        }

        [Fact]
        public async Task Favicon_is_copied_to_the_root()
        {
            var path = "/input/assets/images/favicon.ico";

            var fileProvider = GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[nameof(ImagesPipeline)][Phase.Output].Single();

            Assert.Equal("favicon.ico", document.Destination);
        }

        [Fact]
        public async Task Post_image_in_the_post_folder_is_copied_to_the_correct_folder()
        {
            var path = "/input/posts/2021/2021-11-19-slug/cover.jpg";

            var fileProvider = GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[nameof(ImagesPipeline)][Phase.Output].Single();

            Assert.Equal("assets/images/posts/slug/cover.jpg", document.Destination);
        }

        [Fact]
        public async Task Post_image_in_a_images_folder_under_the_post_folder_is_copied_to_the_correct_folder()
        {
            var path = "/input/posts/2021/2021-11-19-slug/images/cover.jpg";

            var fileProvider = GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[nameof(ImagesPipeline)][Phase.Output].Single();

            Assert.Equal("assets/images/posts/slug/cover.jpg", document.Destination);
        }


        [Fact]
        public async Task Page_image_in_the_post_folder_is_copied_to_the_correct_folder()
        {
            var path = "/input/pages/slug/cover.jpg";

            var fileProvider = GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[nameof(ImagesPipeline)][Phase.Output].Single();

            Assert.Equal("assets/images/pages/slug/cover.jpg", document.Destination);
        }

        [Fact]
        public async Task Page_image_in_a_images_folder_under_the_page_folder_is_copied_to_the_correct_folder()
        {
            var path = "/input/pages/slug/images/cover.jpg";

            var fileProvider = GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[nameof(ImagesPipeline)][Phase.Output].Single();

            Assert.Equal("assets/images/pages/slug/cover.jpg", document.Destination);
        }

        [Fact]
        public async Task All_images_are_copied_to_the_correct_folder()
        {
            var path = "/input/assets/images/cover.jpg";

            var fileProvider = GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[nameof(ImagesPipeline)][Phase.Output].Single();

            Assert.Equal("assets/images/cover.jpg", document.Destination);
        }

        private TestFileProvider GetFileProvider(NormalizedPath path)
            => new TestFileProvider
            {
                path,
            };
    }
}
