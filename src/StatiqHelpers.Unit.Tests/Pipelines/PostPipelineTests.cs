using System;
using System.Linq;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Feeds;
using Statiq.Testing;
using StatiqHelpers.Extensions;
using StatiqHelpers.Pipelines;
using VerifyXunit;
using Xunit;
using xUnitHelpers;
using Assert = Xunit.Assert;

namespace StatiqHelpers.Unit.Tests.Pipelines
{
    [UsesVerify]
    public class PostPipelineTests : BaseFixture
    {
        private readonly Bootstrapper _bootstrapper;
        private readonly NormalizedPath _path;
        private readonly string _content;
        private readonly string _slug;
        private readonly DateTime _publishedDate;

        public PostPipelineTests()
        {
            BaseSetUp();

            _slug = "this-will-be-the-slug";
            _publishedDate = DateTime.SpecifyKind(new DateTime(2021, 11, 19), DateTimeKind.Utc);
            _path = $"/input/posts/{_publishedDate.Year}/{_publishedDate:yyyy-MM-dd}-{_slug}/filename.md";
            _content = @"
---
Article Content 
";

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
            var document = result.Outputs[nameof(PostPipeline)][Phase.PostProcess].Single();

            AssertHelper.AssertMultiple(() => Assert.Equal(title, document.GetTitle()), () => Assert.Equal(excerpt, document.GetExcerpt()));
        }

        [Fact]
        public async Task Sets_metadata_from_path()
        {
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(_path, _content);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[nameof(PostPipeline)][Phase.PostProcess].Single();

            AssertHelper.AssertMultiple(
                () => Assert.Equal(_slug, document.GetSlug()),
                () => Assert.Equal(_publishedDate, document.GetPublishedDate()),
                () => Assert.Equal($"/assets/images/social/{_slug}-facebook.png", document.GetImageFacebook()),
                () => Assert.Equal($"/assets/images/social/{_slug}-twitter.png", document.GetImageTwitter()));
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
            var outputs = result.Outputs[nameof(PostPipeline)];

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
            var outputs = result.Outputs[nameof(PostPipeline)];

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
            var outputs = result.Outputs[nameof(PostPipeline)];

            AssertHelper.AssertMultiple(() => Assert.Single(outputs[Phase.PostProcess]), () => Assert.Single(outputs[Phase.Output]));
        }

        [Fact]
        public async Task Sets_rss_metadata()
        {
            var title = "This is the title";
            var excerpt = "This is the excerpt";
            var coverImage = @"image.jpg";
            var content = $@"
title: {title}
excerpt: {excerpt}
coverImage: ./{coverImage}
---
Article Content 
";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(_path, content);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[nameof(PostPipeline)][Phase.PostProcess].Single();

            AssertHelper.AssertMultiple(
                () => Assert.Equal(excerpt, document.Get(FeedKeys.Description)),
                () => Assert.Equal(_publishedDate, document.Get(FeedKeys.Published)),
                () => Assert.Equal(_publishedDate, document.Get(FeedKeys.Updated)),
                () => Assert.Equal($"/assets/images/posts/{_slug}/{coverImage}", document.Get(FeedKeys.Image)));
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
            var document = result.Outputs[nameof(PostPipeline)][Phase.PostProcess].Single();
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
            var document = result.Outputs[nameof(PostPipeline)][Phase.PostProcess].Single();
            var body = await document.GetContentStringAsync();
            await Verifier.Verify(body);
        }

        [Fact]
        public async Task Sets_reading_time_from_content()
        {
            var body = string.Concat(Enumerable.Repeat("a ", 200));
            var content = $@"
---
{body} 
";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(_path, content);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[nameof(PostPipeline)][Phase.PostProcess].Single();

            Assert.Equal(1, document.GetReadingTime().RoundedMinutes);
        }

        [Fact]
        public async Task Code_blocks_with_language_are_highlighted()
        {
            var content = @"
---
```csharp
int foo = 1
```
";

            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(_path, content);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[nameof(PostPipeline)][Phase.Process].Single();

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
            var document = result.Outputs[nameof(PostPipeline)][Phase.Process].Single();

            var body = await document.GetContentStringAsync();
            await Verifier.Verify(body);
        }

        [Fact]
        public async Task Destination_is_taken_from_the_slug_and_is_placed_in_the_blog_path()
        {
            var slug = "This is tHe Slug With MiXeD CapS";
            var path = $"/input/posts/{_publishedDate.Year}/{_publishedDate:yyyy-MM-dd}-{slug}/FileName With MiXeD CapS.md";
            var fileProvider = PipelineTestHelpersStatic.GetFileProvider(path, _content);

            var result = await _bootstrapper.RunTestAsync(fileProvider);

            Assert.Equal((int)ExitCode.Normal, result.ExitCode);
            var document = result.Outputs[nameof(PostPipeline)][Phase.Output].Single();
            Assert.Equal("blog/this-is-the-slug-with-mixed-caps.html", document.Destination);
        }
    }
}
