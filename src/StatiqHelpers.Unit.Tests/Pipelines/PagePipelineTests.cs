using System.Linq;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Extensions;
using StatiqHelpers.Pipelines;
using VerifyXunit;
using Xunit;
using xUnitHelpers;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    [UsesVerify]
    public class PagePipelineTests : BaseFixture
    {
        private const string PipelineName = nameof(PagesPipeline);
        private readonly Bootstrapper _bootstrapper;
        private readonly NormalizedPath _path;
        private readonly string _slug;

        public PagePipelineTests()
        {
            BaseSetUp();

            _slug = "slug";
            _path = $"/input/pages/{_slug}/filename.md";

            _bootstrapper = PipelineTestHelpersStatic.GetBootstrapper();
        }

        [Fact]
        public async Task Sets_metadata_from_front_matter()
        {
            var title = "This is the title";
            var excerpt = "This is the excerpt";

            var content = $@"
title: {title}
excerpt: {excerpt}
---
Article Content 
";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(_path, content);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.PostProcess].Single();

            AssertHelper.AssertMultiple(() => Assert.Equal(title, document.GetTitle()), () => Assert.Equal(excerpt, document.GetExcerpt()));
        }

        [Fact]
        public async Task Sets_metadata_from_path()
        {
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(_path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.PostProcess].Single();

            AssertHelper.AssertMultiple(
                () => Assert.Equal(_slug, document.GetSlug()),
                () => Assert.Equal($"/assets/images/social/{_slug}-facebook.png", document.GetImageFacebook()),
                () => Assert.Equal($"/assets/images/social/{_slug}-twitter.png", document.GetImageTwitter()));
        }

        [Fact]
        public async Task Image_links_are_created_correctly_for_images_in_the_page_folder()
        {
            var content = @"
---
![Alt text 1](./image1.jpg)

![Alt text 2](./image2.jpg)
";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(_path, content);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.PostProcess].Single();
            var body = await document.GetContentStringAsync();
            await Verifier.Verify(body);
        }

        [Fact]
        public async Task Image_links_are_created_correctly_for_images_in_a_images_folder_under_the_page_folder()
        {
            var content = @"
---
![Alt text 1](./images/image1.jpg)

![Alt text 2](./images/image2.jpg)
";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(_path, content);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.PostProcess].Single();
            var body = await document.GetContentStringAsync();
            await Verifier.Verify(body);
        }

        [Fact]
        public async Task Destination_is_taken_from_the_slug_for_markdown_files()
        {
            var slug = "folder 1/Slug MiXeD CapS";
            var path = $"/input/pages/{slug}/FileName.md";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();
            Assert.Equal("folder-1/slug-mixed-caps.html", document.Destination);
        }

        [Fact]
        public async Task Files_with_cshtml_extension_set_the_destination_from_slug_and_filename()
        {
            var path = $"/input/pages/{_slug}/filename.cshtml";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();
            Assert.Equal($"{_slug}/filename.html", document.Destination);
        }
    }
}
