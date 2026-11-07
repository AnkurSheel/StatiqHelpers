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
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 85);
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

                var targetWidth = newWidth;
                var targetHeight = newHeight;

                if (targetWidth == 0 && targetHeight == 0)
                {
                    targetWidth = bitmap.Width;
                    targetHeight = bitmap.Height;
                }
                else if (targetWidth == 0)
                {
                    targetWidth = (int)(bitmap.Width * (float)targetHeight / bitmap.Height);
                }
                else if (targetHeight == 0)
                {
                    targetHeight = (int)(bitmap.Height * (float)targetWidth / bitmap.Width);
                }

                if (!increaseImageSizes && bitmap.Width < targetWidth && bitmap.Height < targetHeight)
                {
                    continue;
                }

                var oldWidth = bitmap.Width;
                var oldHeight = bitmap.Height;
                var fileInfo = new FileInfo(path);
                var oldSize = fileInfo.Length;

                using var resized = new SKBitmap(targetWidth, targetHeight);
                bitmap.ScalePixels(resized, SKFilterQuality.High);
                using var image = SKImage.FromBitmap(resized);
                if (image == null)
                {
                    continue;
                }

                var extension = Path.GetExtension(path).ToLowerInvariant();
                var targetExtension = extension switch
                {
                    ".jpg" or ".jpeg" => ".jpg",
                    _ => ".webp"
                };
                var (format, quality) = targetExtension switch
                {
                    ".jpg" => (SKEncodedImageFormat.Jpeg, 80),
                    _ => (SKEncodedImageFormat.Webp, 80)
                };

                var targetPath = Path.ChangeExtension(path, targetExtension);

                using var data = image.Encode(format, quality);
                await using (var stream = File.OpenWrite(targetPath))
                {
                    stream.SetLength(0);
                    data.SaveTo(stream);
                }

                if (!string.Equals(path, targetPath, StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(path);
                }

                var targetFileInfo = new FileInfo(targetPath);
                var newSize = targetFileInfo.Length;

                _logger.LogInformation(
                    "Resized {Path}: dimensions {OldWidth}x{OldHeight} -> {NewWidth}x{NewHeight}, size on disk {OldSize} -> {NewSize}",
                    targetPath,
                    oldWidth,
                    oldHeight,
                    targetWidth,
                    targetHeight,
                    oldSize,
                    newSize
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Resize failed for {path}", path);
            }
        }
    }
}
