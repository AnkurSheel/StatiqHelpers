using Statiq.Common;
using Statiq.Feeds;
using Statiq.Testing;
using StatiqHelpers.Modules;
using xUnitHelpers;

namespace StatiqHelpers.Unit.Tests.Modules
{
    public class GenerateRssMetaDataTests : BaseFixture
    {
        private const string Content = "<p>Some Content</p>";
        private const string Title = "Title of post";
        private const string Excerpt = "An excerpt for the post";
        private const string Slug = "post-slug";
        private readonly TestDocument _input;
        private readonly GenerateRssMetaData _module;
        private readonly DateTime _publishedDate;

        public GenerateRssMetaDataTests()
        {
            _input = new TestDocument(new NormalizedPath("a.txt"), Content);
            _module = new GenerateRssMetaData();
            _publishedDate = DateTime.SpecifyKind(new DateTime(2022, 2, 19), DateTimeKind.Utc);

            _input.Add(Keys.Title, Title);
            _input.Add(MetaDataKeys.Excerpt, Excerpt);
            _input.Add(MetaDataKeys.PublishedDate, _publishedDate);
            _input.Add(MetaDataKeys.Slug, Slug);
        }

        [Fact]
        public async Task Properties_are_set_correctly()
        {
            var result = await ExecuteAsync(_input, _module).SingleAsync();

            AssertProperties(result);
        }

        [Fact]
        public async Task Image_is_taken_from_post_cover_image()
        {
            const string imageFileName = "cover.jpg";

            _input.Add(MetaDataKeys.CoverImage, $"./{imageFileName}");

            var result = await ExecuteAsync(_input, _module).SingleAsync();

            AssertProperties(result, $"/assets/images/posts/{Slug}/{imageFileName}");
        }

        [Fact]
        public async Task Image_is_taken_from_images_directory_when_post_cover_image_path_is_in_the_images_directory()
        {
            const string imageFileName = "assets/images/cover.jpg";
            _input.Add(MetaDataKeys.CoverImage, $"./{imageFileName}");

            var result = await ExecuteAsync(_input, _module).SingleAsync();

            AssertProperties(result, imageFileName);
        }

        [Fact]
        public async Task Updated_date_is_taken_from_post_updated_date()
        {
            var updatedDate = DateTime.SpecifyKind(new DateTime(2022, 2, 20), DateTimeKind.Utc);

            _input.Add(MetaDataKeys.UpdatedOnDate, updatedDate);

            var result = await ExecuteAsync(_input, _module).SingleAsync();

            AssertProperties(result, updatedDate: updatedDate);
        }

        private void AssertProperties(TestDocument result, string? expectedImageLink = null, DateTime? updatedDate = null)
        {
            updatedDate ??= _publishedDate;
            AssertHelper.AssertMultiple(
                () => Assert.Equal(Title, result[FeedKeys.Title]),
                () => Assert.Equal(Excerpt, result[FeedKeys.Description]),
                () => Assert.Equal(expectedImageLink, result[FeedKeys.Image]),
                () => Assert.Equal(_publishedDate, result[FeedKeys.Published]),
                () => Assert.Equal(updatedDate, result[FeedKeys.Updated]),
                () => Assert.Equal(Content, result.Content));
        }
    }
}
