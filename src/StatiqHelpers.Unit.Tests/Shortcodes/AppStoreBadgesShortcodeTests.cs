using Statiq.Testing;
using StatiqHelpers.Shortcodes;

namespace StatiqHelpers.Unit.Tests.Shortcodes;

[UsesVerify]
public class AppStoreBadgesShortcodeTests : BaseFixture
{
    private readonly TestExecutionContext _context = new();
    private readonly TestDocument _document = new();
    private readonly AppStoreBadgesShortcode _shortcode = new();

    [Theory]
    [InlineData("Get Game 1", "game_1/id100")]
    [InlineData("Get Game 2", "game_2/id200")]
    public async Task Can_render_the_markup_for_app_store_badges(string appStoreLinkText, string appStoreLinkUrl)
    {
        var settings = new VerifyTests.VerifySettings();
        settings.UseParameters(appStoreLinkText, appStoreLinkUrl);
        settings.UseExtension("html");

        KeyValuePair<string, string>[] args =
        [
            new("AppStoreLinkText", appStoreLinkText),
            new("AppStoreLinkUrl", appStoreLinkUrl)
        ];

        var result = _shortcode.Execute(args, string.Empty, _document, _context);

        await Verify(result.ContentProvider.GetStream().ReadToEnd(), settings);
    }

    [Theory]
    [InlineData("Get Game 1", "com.example.app1")]
    [InlineData("Get Game 2", "com.example.app2")]
    public async Task Can_render_the_markup_for_play_store_badges(string playStoreLinkText, string playStoreLinkUrl)
    {
        var settings = new VerifyTests.VerifySettings();
        settings.UseParameters(playStoreLinkText, playStoreLinkUrl);
        settings.UseExtension("html");

        KeyValuePair<string, string>[] args =
        [
            new("GooglePlayLinkText", playStoreLinkText),
            new("GooglePlayLinkUrl", playStoreLinkUrl)
        ];

        var result = _shortcode.Execute(args, string.Empty, _document, _context);

        await Verify(result.ContentProvider.GetStream().ReadToEnd(), settings);
    }

    [Fact]
    public async Task Can_render_the_markup_for_app_store_and_google_play_badges()
    {
        var settings = new VerifyTests.VerifySettings();
        settings.UseExtension("html");

        KeyValuePair<string, string>[] args =
        [
            new("AppStoreLinkText", "Get Game"),
            new("AppStoreLinkUrl", "game_1/id100"),
            new("GooglePlayLinkText", "Get Game"),
            new("GooglePlayLinkUrl", "com.app1")
        ];

        var result = _shortcode.Execute(args, string.Empty, _document, _context);

        await Verify(result.ContentProvider.GetStream().ReadToEnd(), settings);
    }

    [Fact]
    public void Throws_if_neither_app_store_nor_google_play_args_are_provided()
    {
        // No arguments at all
        var args = Array.Empty<KeyValuePair<string, string>>();
        Assert.Throws<ArgumentException>(() => _shortcode.Execute(args, string.Empty, _document, _context));
    }

    [Fact]
    public void Throws_if_only_partial_app_store_args_are_provided()
    {
        // Only AppStoreLinkText
        var args = new[]
        {
            new KeyValuePair<string, string>("AppStoreLinkText", "App Store")
        };
        Assert.Throws<ArgumentException>(() => _shortcode.Execute(args, string.Empty, _document, _context));
        // Only AppStoreLinkUrl
        args =
        [
            new KeyValuePair<string, string>("AppStoreLinkUrl", "id123")
        ];
        Assert.Throws<ArgumentException>(() => _shortcode.Execute(args, string.Empty, _document, _context));
    }

    [Fact]
    public void Throws_if_only_partial_google_play_args_are_provided()
    {
        // Only GooglePlayLinkText
        var args = new[]
        {
            new KeyValuePair<string, string>("GooglePlayLinkText", "Google Play")
        };
        Assert.Throws<ArgumentException>(() => _shortcode.Execute(args, string.Empty, _document, _context));
        // Only GooglePlayLinkUrl
        args =
        [
            new KeyValuePair<string, string>("GooglePlayLinkUrl", "com.example.app")
        ];
        Assert.Throws<ArgumentException>(() => _shortcode.Execute(args, string.Empty, _document, _context));
    }
}
