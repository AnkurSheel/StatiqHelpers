namespace StatiqHelpers.Models
{
    public class NavigationLink
    {
        public NavigationLink(string title, string relativeUrl)
        {
            Title = title;
            RelativeUrl = relativeUrl;
        }

        public string Title { get; }

        public string RelativeUrl { get; }
    }
}
