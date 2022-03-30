using System.Xml.Linq;
using Statiq.Common;

namespace StatiqHelpers.Shortcodes
{
    public class ResponsiveYouTubeShortcode : SyncShortcode
    {
        private const string Id = nameof(Id);
        private const string Title = nameof(Title);

        public override ShortcodeResult Execute(
            KeyValuePair<string, string>[] args,
            string content,
            IDocument document,
            IExecutionContext context)
        {
            IMetadataDictionary arguments = args.ToDictionary(Id, Title);
            arguments.RequireKeys(Id, Title);

            XElement container = new XElement("div", new XAttribute("style", "position: relative;width: 100%;padding-bottom: 56.25%;margin: 1rem 0;"));

            return new XElement(
                "div",
                new XAttribute("style", "position: relative;width: 100%;padding-bottom: 56.25%;margin: 1rem 0;"),
                new XElement(
                    "iframe",
                    string.Empty,
                    new XAttribute("src", $"https://www.youtube.com/embed/{arguments.GetString(Id)}"),
                    new XAttribute("title", $"{arguments.GetString(Title)}"),
                    new XAttribute("style", "position: absolute;width: 100%;height: 100%;left: 0;top: 0;right: 0;"),
                    new XAttribute("allowfullscreen", string.Empty))).ToString();
        }
    }
}
