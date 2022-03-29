using Statiq.Testing;
using StatiqHelpers.Shortcodes;

namespace StatiqHelpers.Unit.Tests.Shortcodes
{
    [UsesVerify]
    public class ResponsiveYouTubeShortcodeTests : BaseFixture
    {
        private readonly TestExecutionContext _context = new TestExecutionContext();
        private readonly TestDocument _document = new TestDocument();

        [Fact]
        public void If_required_arguments_are_not_passed_throw_exception()
        {
            var args = Array.Empty<KeyValuePair<string, string>>();
            var shortcode = new ResponsiveYouTubeShortcode();

            Assert.Throws<ArgumentException>(() => shortcode.Execute(args, "foo bar", _document, _context));
        }

        [Theory]
        [InlineData("O0jWcGCj-aA", "Dubai Fountain")]
        [InlineData("xC1Sh-m6Qu4", "Dubai Fountain 1")]
        public async Task Can_render_the_markup_for_responsive_youtube_videos(string id, string title)
        {
            var settings = new VerifyTests.VerifySettings();
            settings.UseParameters(id, title);
            settings.UseExtension("html");

            KeyValuePair<string, string>[] args =
            {
                new KeyValuePair<string, string>("Id", id),
                new KeyValuePair<string, string>("Title", title),
            };
            var shortcode = new ResponsiveYouTubeShortcode();

            var result = shortcode.Execute(args, string.Empty, _document, _context);

            await Verifier.Verify(result.ContentProvider.GetStream().ReadToEnd(), settings);
        }
    }
}
