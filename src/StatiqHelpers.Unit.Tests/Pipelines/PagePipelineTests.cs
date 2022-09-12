using Statiq.Testing;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    [UsesVerify]
    public class PagePipelineTests : PipelineBaseFixture
    {
        private const string PipelineName = nameof(PagesPipeline);
        private readonly NormalizedPath _path;
        private readonly string _slug;

        public PagePipelineTests()
        {
            _slug = "slug";
            _path = $"/input/pages/{_slug}/filename.md";
        }

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
        public async Task Image_links_are_created_correctly_for_images_in_the_page_folder()
        {
            var content = @"
---
![Alt text 1](./image1.jpg)

![Alt text 2](./image2.jpg)
";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(_path, content);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.PostProcess].Single();
            var body = await document.GetContentStringAsync();
            await Verify(body);
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

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.PostProcess].Single();
            var body = await document.GetContentStringAsync();
            await Verify(body);
        }

        [Fact]
        public async Task Destination_is_taken_from_the_slug_for_markdown_files()
        {
            var slug = "folder 1/Slug MiXeD CapS";
            var path = $"/input/pages/{slug}/FileName.md";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();
            Assert.Equal("folder-1/slug-mixed-caps.html", document.Destination.ToString());
        }

        [Fact]
        public async Task Files_with_cshtml_extension_set_the_destination_from_slug_and_filename()
        {
            var path = $"/input/pages/{_slug}/filename.cshtml";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path);

            var result = await Bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();
            Assert.Equal($"{_slug}/filename.html", document.Destination.ToString());
        }
    }
}
