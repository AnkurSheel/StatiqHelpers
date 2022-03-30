namespace StatiqHelpers.ImageHelpers
{
    public interface IImageService
    {
        Task<Stream> CreateImageDocument(
            int width,
            int height,
            string? coverImagePath,
            string siteTitle,
            string centerText);

        Task ResizeImages(IReadOnlyList<string> imagePaths, int newWidth, int newHeight);
    }
}
