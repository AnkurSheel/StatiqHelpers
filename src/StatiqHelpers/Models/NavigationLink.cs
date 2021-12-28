namespace StatiqHelpers.Models
{
    public class NavigationLink
    {
        public NavigationLink(string title, string url)
        {
            Title = title;
            Url = url;
        }

        public string Title { get; }

        public string Url { get; }
    }
}
