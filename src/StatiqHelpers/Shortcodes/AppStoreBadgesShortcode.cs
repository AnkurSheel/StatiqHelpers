using System.Xml.Linq;

namespace StatiqHelpers.Shortcodes
{
    public class AppStoreBadgesShortcode : SyncShortcode
    {
        private const string AppStoreLinkText = nameof(AppStoreLinkText);
        private const string AppStoreLinkUrl = nameof(AppStoreLinkUrl);
        private const string GooglePlayLinkText = nameof(GooglePlayLinkText);
        private const string GooglePlayLinkUrl = nameof(GooglePlayLinkUrl);

        public override ShortcodeResult Execute(
            KeyValuePair<string, string>[] args,
            string content,
            IDocument document,
            IExecutionContext context)
        {
            var arguments = args.ToDictionary(AppStoreLinkText, AppStoreLinkUrl, GooglePlayLinkText, GooglePlayLinkUrl);

            var hasAppStore = !string.IsNullOrEmpty(arguments.GetString(AppStoreLinkText))
                              && !string.IsNullOrEmpty(arguments.GetString(AppStoreLinkUrl));
            var hasGooglePlay = !string.IsNullOrEmpty(arguments.GetString(GooglePlayLinkText))
                                && !string.IsNullOrEmpty(arguments.GetString(GooglePlayLinkUrl));

            if (!hasAppStore && !hasGooglePlay)
            {
                throw new ArgumentException(
                    "At least one of App Store or Google Play badge arguments must be provided: (LinkText + AppStoreLinkUrl) or (GooglePlayLinkText + GooglePlayLinkUrl)");
            }

            var containerDiv = new XElement("div", new XAttribute("class", "appstore-badges-container"));

            // Add App Store badge if URL is provided
            if (arguments.ContainsKey(AppStoreLinkUrl) && !string.IsNullOrEmpty(arguments.GetString(AppStoreLinkUrl)))
            {
                var appStoreLink = CreateBadgeLink(
                    $"https://apps.apple.com/in/app/{arguments.GetString(AppStoreLinkUrl)}",
                    "/assets/images/download-app-store-badge.png",
                    arguments.GetString(AppStoreLinkText));
                containerDiv.Add(appStoreLink);
            }

            // Add Google Play badge if URL is provided
            if (arguments.ContainsKey(GooglePlayLinkUrl)
                && !string.IsNullOrEmpty(arguments.GetString(GooglePlayLinkUrl)))
            {
                var playStoreLink = CreateBadgeLink(
                    $"https://play.google.com/store/apps/details?id={arguments.GetString(GooglePlayLinkUrl)}",
                    "/assets/images/download-play-store-badge.png",
                    arguments.GetString(GooglePlayLinkText));
                containerDiv.Add(playStoreLink);
            }

            return containerDiv.ToString();
        }

        private XElement CreateBadgeLink(string url, string imageSrc, string altText)
            => new(
                "a",
                new XAttribute("href", url),
                new XAttribute("target", "_blank"),
                new XElement("img", new XAttribute("src", imageSrc), new XAttribute("alt", altText)));
    }
}
