using System.Xml.Linq;

namespace StatiqHelpers.Shortcodes
{
    public class AppStoreBadgesShortcode : SyncShortcode
    {
        private const string LinkText = nameof(LinkText);
        private const string AppStoreLinkUrl = nameof(AppStoreLinkUrl);

        public override ShortcodeResult Execute(
            KeyValuePair<string, string>[] args,
            string content,
            IDocument document,
            IExecutionContext context)
        {
            IMetadataDictionary arguments = args.ToDictionary(LinkText, AppStoreLinkUrl);
            arguments.RequireKeys(LinkText, AppStoreLinkUrl);

            XElement linkElement = new XElement(
                "a",
                new XAttribute("href", $"https://apps.apple.com/us/app/{arguments.GetString(AppStoreLinkUrl)}"),
                new XAttribute("target", "_blank"),
                new XElement(
                    "img",
                    new XAttribute("src", "/assets/images/download-app-store-badge.png"),
                    new XAttribute("alt", arguments.GetString(LinkText))));

            return linkElement.ToString();
        }
    }
}