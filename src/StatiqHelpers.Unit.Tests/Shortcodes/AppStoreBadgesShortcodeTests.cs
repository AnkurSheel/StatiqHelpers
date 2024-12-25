using Statiq.Testing;
using StatiqHelpers.Shortcodes;

namespace StatiqHelpers.Unit.Tests.Shortcodes;

[UsesVerify]
public class AppStoreBadgesShortcodeTests : BaseFixture
{
    private readonly TestExecutionContext _context = new TestExecutionContext();
    private readonly TestDocument _document = new TestDocument();
    private readonly AppStoreBadgesShortcode _shortcode = new();

    [Fact]
    public void If_required_arguments_are_not_passed_throw_exception()
    {
        var args = Array.Empty<KeyValuePair<string, string>>();

        Assert.Throws<ArgumentException>(() => _shortcode.Execute(args, "foo bar", _document, _context));
    }

    [Theory]
    [InlineData("Get Game 1", "game_1/id100")]
    [InlineData("Get Game 2", "game_2/id200")]
    public async Task Can_render_the_markup_for_app_store_badges(string linkText, string appStoreLinkUrl)
    {
        var settings = new VerifyTests.VerifySettings();
        settings.UseParameters(linkText, appStoreLinkUrl);
        settings.UseExtension("html");

        KeyValuePair<string, string>[] args =
        {
            new KeyValuePair<string, string>("LinkText", linkText),
            new KeyValuePair<string, string>("AppStoreLinkUrl", appStoreLinkUrl),
        };

        var result = _shortcode.Execute(args, string.Empty, _document, _context);

        await Verify(result.ContentProvider.GetStream().ReadToEnd(), settings);
    }
}