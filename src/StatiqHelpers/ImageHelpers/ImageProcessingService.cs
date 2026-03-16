using Microsoft.Extensions.Logging;

using SkiaSharp;

namespace StatiqHelpers.ImageHelpers;

public class ImageProcessingService : IImageProcessingService
{
    private readonly ILogger<ImageProcessingService> _logger;

    public ImageProcessingService(ILogger<ImageProcessingService> logger)
    {
        _logger = logger;
    }

    public async Task<Stream> EncodeToStream(SKSurface surface)
    {
        var output = new MemoryStream();
        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        data.SaveTo(output);
        output.Seek(0, SeekOrigin.Begin);
        return await Task.FromResult(output);
    }

    public SKBitmap ResizeBitmap(int width, int height, SKBitmap original)
    {
        var resized = new SKBitmap(width, height);
        original.ScalePixels(resized, SKFilterQuality.High);
        return resized;
    }

    public async Task ResizeImages(
        IReadOnlyList<string> imagePaths,
        int newWidth,
        int newHeight,
        bool increaseImageSizes
    )
    {
        foreach (var path in imagePaths)
        {
            try
            {
                using var bitmap = SKBitmap.Decode(path);
                if (bitmap == null)
                {
                    continue;
                }

                if (!increaseImageSizes && bitmap.Width <= newWidth && (newHeight == 0 || bitmap.Height <= newHeight))
                {
                    continue;
                }

                using var resized = new SKBitmap(
                    newWidth,
                    newHeight == 0 ? (int)(bitmap.Height * (float)newWidth / bitmap.Width) : newHeight);
                bitmap.ScalePixels(resized, SKFilterQuality.High);
                using var image = SKImage.FromBitmap(resized);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                await using var stream = File.OpenWrite(path);
                stream.SetLength(0);
                data.SaveTo(stream);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Resize failed for {path}", path);
            }
        }
    }
}
