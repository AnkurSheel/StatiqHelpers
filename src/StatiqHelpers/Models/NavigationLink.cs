namespace StatiqHelpers.Models
{
    public class NavigationLink
    {
        public static NavigationLink NavigationLinkWithChildren(string title, IReadOnlyList<NavigationLink> childLinks)
            => new NavigationLink(title, null, childLinks);

        public static NavigationLink NavigationLinkWithoutChildren(string title, string url)
            => new NavigationLink(title, url, null);

        private NavigationLink(string title, string? url, IReadOnlyList<NavigationLink>? childLinks)
        {
            Title = title;
            Url = url;
            ChildLinks = childLinks;
        }

        public string Title { get; }

        public string? Url { get; }

        public IReadOnlyList<NavigationLink>? ChildLinks { get; }
    }
}
