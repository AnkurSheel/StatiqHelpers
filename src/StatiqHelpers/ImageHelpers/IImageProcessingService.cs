using SkiaSharp;

namespace StatiqHelpers.ImageHelpers;

public interface IImageProcessingService
{
    Task<Stream> EncodeToStream(SKSurface surface);

    SKBitmap ResizeBitmap(int width, int height, SKBitmap original);

    Task ResizeImages(IReadOnlyList<string> imagePaths, int newWidth, int newHeight, bool increaseImageSizes);
}
