using System;
using System.Linq;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Testing;
using StatiqHelpers.Pipelines;
using VerifyXunit;
using Xunit;
using xUnitHelpers;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    [UsesVerify]
    public class PostPipelineTests : BaseFixture
    {
        private const string PipelineName = nameof(PostPipeline);
        private readonly Bootstrapper _bootstrapper;
        private readonly NormalizedPath _path;
        private readonly string _content;
        private readonly string _slug;
        private readonly DateTime _publishedDate;

        public PostPipelineTests()
        {
            BaseSetUp();

            _slug = "slug";
            _publishedDate = DateTime.SpecifyKind(new DateTime(2021, 11, 19), DateTimeKind.Utc);
            _path = $"/input/posts/{_publishedDate.Year}/{_publishedDate:yyyy-MM-dd}-{_slug}/filename.md";
            _content = @"
---
Article Content 
";

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
        public async Task Verify_process_modules_cache()
        {
            await PipelineCommonTests.Verify_process_modules_cache(_bootstrapper, PipelineName);
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
        public async Task Filters_posts_in_the_future_for_non_development_environments()
        {
            var publishedDate = DateTime.SpecifyKind(DateTime.Today.AddDays(1), DateTimeKind.Utc);
            var path = $"/input/posts/{publishedDate.Year}/{publishedDate:yyyy-MM-dd}-{_slug}/filename.md";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path, _content);

            _bootstrapper.AddSetting("Environment", "Test");
            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var outputs = result.Outputs[PipelineName];

            AssertHelper.AssertMultiple(() => Assert.Empty(outputs[Phase.PostProcess]), () => Assert.Empty(outputs[Phase.Output]));
        }

        [Fact]
        public async Task Does_not_filter_posts_in_the_future_for_development_environment()
        {
            var publishedDate = DateTime.SpecifyKind(DateTime.Today.AddDays(1), DateTimeKind.Utc);
            var path = $"/input/posts/{publishedDate.Year}/{publishedDate:yyyy-MM-dd}-{_slug}/filename.md";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path, _content);

            _bootstrapper.AddSetting("Environment", "Development");
            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var outputs = result.Outputs[PipelineName];

            AssertHelper.AssertMultiple(() => Assert.Single(outputs[Phase.PostProcess]), () => Assert.Single(outputs[Phase.Output]));
        }

        [Fact]
        public async Task Does_not_filter_posts_in_the_future_if_environment_setting_does_not_exist()
        {
            var publishedDate = DateTime.SpecifyKind(DateTime.Today.AddDays(1), DateTimeKind.Utc);
            var path = $"/input/posts/{publishedDate.Year}/{publishedDate:yyyy-MM-dd}-{_slug}/filename.md";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path, _content);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var outputs = result.Outputs[PipelineName];

            AssertHelper.AssertMultiple(() => Assert.Single(outputs[Phase.PostProcess]), () => Assert.Single(outputs[Phase.Output]));
        }

        [Fact]
        public async Task Image_links_are_created_correctly_for_images_in_the_post_folder()
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
        public async Task Image_links_are_created_correctly_for_images_in_a_images_folder_under_the_post_folder()
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
        public async Task Code_blocks_without_language_are_highlighted()
        {
            var content = @"
---
```
int foo = 1
```
";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(_path, content);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Process].Single();

            var body = await document.GetContentStringAsync();
            await Verifier.Verify(body);
        }

        [Fact]
        public async Task Destination_is_taken_from_the_slug_and_is_placed_in_the_blog_path()
        {
            var slug = "Slug MiXeD CapS";
            var path = $"/input/posts/{_publishedDate.Year}/{_publishedDate:yyyy-MM-dd}-{slug}/FileName With MiXeD CapS.md";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path, _content);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[PipelineName][Phase.Output].Single();
            Assert.Equal("blog/slug-mixed-caps.html", document.Destination);
        }
    }
}
