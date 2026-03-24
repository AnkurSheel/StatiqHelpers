namespace StatiqHelpers
{
    public static class Constants
    {
        public const string AssetsDirectory = "assets";
        public static readonly string ImagesDirectory = $"{AssetsDirectory}/images";
        public static readonly string JsDirectory = $"{AssetsDirectory}/js";
        public static readonly string PostImagesDirectory = $"{ImagesDirectory}/posts";
        public static readonly string PagesImagesDirectory = $"{ImagesDirectory}/pages";
        public static readonly string SocialImagesDirectory = $"{ImagesDirectory}/social";
        public static readonly string FontsDirectory = $"{AssetsDirectory}/fonts";
        public static readonly string DownloadsDirectory = $"{AssetsDirectory}/downloads";
        public static readonly string BlogPath = "blog";

        public static readonly string DefaultCategory = "Uncategorized";

        public static readonly string[] StopWords =
        {
            "a",
            "an",
            "and",
            "as",
            "from",
            "how",
            "I",
            "in",
            "is",
            "my",
            "of",
            "or",
            "own",
            "the",
            "to",
            "using",
            "why",
            "with",
            "within",
            "your",
        };
    }
}
