using ByteSizeLib;

using Microsoft.Extensions.Logging;

using SkiaSharp;

namespace StatiqHelpers.ImageHelpers
{
    public class ImageService : IImageService
    {
        private readonly IFontHelper _fontHelper;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IFontHelper fontHelper, ILogger<ImageService> logger)
        {
            _fontHelper = fontHelper;
            _logger = logger;
        }

        public async Task<Stream> CreateImageDocument(
            int width,
            int height,
            string? coverImagePath,
            string siteTitle,
            string centerText,
            string fontPath
        )
        {
            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;

            using var typeface = _fontHelper.InstallFont(fontPath);
            var fontSize = height / 10f;

            AddGradient(width, height, canvas);

            if (coverImagePath != null)
            {
                using var coverImage = SKBitmap.Decode(coverImagePath);
                if (coverImage != null)
                {
                    using var thumbnail = ResizeBitmap(width, height, coverImage);
                    DarkenBitmap(thumbnail);
                    canvas.DrawBitmap(thumbnail, 0, 0);
                }
            }

            AddCenterText(canvas, width, height, centerText, typeface, fontSize);
            AddBrand(canvas, width, height, siteTitle, typeface, fontSize);

            var output = new MemoryStream();
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            data.SaveTo(output);
            output.Seek(0, SeekOrigin.Begin);

            return await Task.FromResult(output);
        }

        private SKBitmap ResizeBitmap(int width, int height, SKBitmap original)
        {
            var resized = new SKBitmap(width, height);
            original.ScalePixels(resized, SKFilterQuality.High);
            return resized;
        }

        private void DarkenBitmap(SKBitmap bitmap)
        {
            using var canvas = new SKCanvas(bitmap);
            using var paint = new SKPaint
            {
                Color = SKColors.Black.WithAlpha((byte)(255 * 0.5)),
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRect(0, 0, bitmap.Width, bitmap.Height, paint);
        }

        public async Task ResizeImages(
            IReadOnlyList<string> imagePaths,
            int newWidth,
            int newHeight,
            bool increaseImageSizes
        )
        {
            var totalPre = 0L;
            var totalPost = 0L;

            var skippedImages = 0;

            foreach (var imagePath in imagePaths)
            {
                var fileInfo = new FileInfo(imagePath);
                var preSize = 0L;
                SKBitmap bitmap;
                try
                {
                    bitmap = SKBitmap.Decode(imagePath);
                    if (bitmap == null)
                    {
                        throw new Exception("Failed to decode image");
                    }
                }
                catch (Exception e)
                {
                    _logger.Log(LogLevel.Error, "Error loading image {Path}: {Message}", imagePath, e.Message);
                    continue;
                }

                using (bitmap)
                {
                    var originalWidth = bitmap.Width;
                    var originalHeight = bitmap.Height;

                    if (originalWidth == newWidth && (newHeight == 0 || originalHeight == newHeight)
                        || originalHeight == newHeight && (newWidth == 0 || originalWidth == newWidth))
                    {
                        continue;
                    }

                    if ((newWidth > originalWidth || newHeight > originalHeight) && !increaseImageSizes)
                    {
                        skippedImages++;
                        continue;
                    }

                    preSize = fileInfo.Length;
                    totalPre += preSize;

                    var finalWidth = newWidth;
                    var finalHeight = newHeight;

                    if (newHeight == 0)
                    {
                        finalHeight = (int)(originalHeight * (float)newWidth / originalWidth);
                    }
                    else if (newWidth == 0)
                    {
                        finalWidth = (int)(originalWidth * (float)newHeight / originalHeight);
                    }

                    using var resizedBitmap = new SKBitmap(finalWidth, finalHeight);
                    bitmap.ScalePixels(resizedBitmap, SKFilterQuality.High);

                    SKEncodedImageFormat format;
                    int quality = 80;

                    switch (fileInfo.Extension.ToLower())
                    {
                        case ".jpg":
                        case ".jpeg":
                            format = SKEncodedImageFormat.Jpeg;
                            break;
                        case ".png":
                            format = SKEncodedImageFormat.Png;
                            quality = 100;
                            break;
                        default:
                            _logger.Log(
                                LogLevel.Error,
                                "No support for extension {Extension} in {Path}",
                                fileInfo.Extension,
                                imagePath);
                            continue;
                    }

                    using var image = SKImage.FromBitmap(resizedBitmap);
                    using var data = image.Encode(format, quality);
                    using var stream = File.OpenWrite(imagePath);
                    stream.SetLength(0);
                    data.SaveTo(stream);
                }

                fileInfo = new FileInfo(imagePath);
                var postSize = fileInfo.Length;
                totalPost += postSize;

                var percentChanged = Math.Abs(postSize - preSize) / (decimal)preSize;

                _logger.Log(
                    LogLevel.Information,
                    "Resized {Path} with {Size} to {NewSize}. Changed from {PreSize} to {PostSize} ({PercentChanged:P})",
                    imagePath,
                    $"{bitmap.Width}x{bitmap.Height}",
                    $"{newWidth}x{newHeight}",
                    ByteSize.FromBytes(preSize).ToString(),
                    ByteSize.FromBytes(postSize).ToString(),
                    percentChanged);
            }

            _logger.Log(
                LogLevel.Information,
                "Resizing complete. Updated from {PreSize} to {PostSize}. Skipped Images: {SkippedImages}.",
                ByteSize.FromBytes(totalPre).ToString(),
                ByteSize.FromBytes(totalPost).ToString(),
                skippedImages);
        }

        private void AddGradient(int width, int height, SKCanvas canvas)
        {
            using var paint = new SKPaint
            {
                Shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    new SKPoint(width / 2f, height / 2f),
                    new[] { SKColor.Parse("#16222A"), SKColor.Parse("#3A6073") },
                    new[] { 0f, 0.5f },
                    SKShaderTileMode.Mirror)
            };
            canvas.DrawRect(0, 0, width, height, paint);
        }

        private void AddCenterText(
            SKCanvas canvas,
            int imageWidth,
            int imageHeight,
            string centerText,
            SKTypeface typeface,
            float fontSize
        )
        {
            var xPadding = imageWidth / 30f;
            using var paint = new SKPaint
            {
                Typeface = typeface,
                TextSize = fontSize,
                Color = SKColors.White,
                IsAntialias = true,
                TextAlign = SKTextAlign.Center
            };

            var wrapWidth = imageWidth - xPadding * 2;
            var lines = WrapText(centerText, wrapWidth, paint);

            var totalHeight = lines.Count * fontSize;
            var y = (imageHeight - totalHeight) / 2f + fontSize;

            foreach (var line in lines)
            {
                // Shadow
                paint.Color = SKColors.MediumPurple;
                canvas.DrawText(line, imageWidth / 2f + 2, y + 2, paint);

                // Text
                paint.Color = SKColors.White;
                canvas.DrawText(line, imageWidth / 2f, y, paint);

                y += fontSize;
            }
        }

        private List<string> WrapText(string text, float maxWidth, SKPaint paint)
        {
            var resultLines = new List<string>();

            foreach (var paragraph in text.ReplaceLineEndings().Split(Environment.NewLine))
            {
                var currentLine = string.Empty;

                foreach (var word in paragraph.Split(' '))
                {
                    var testLine = string.IsNullOrWhiteSpace(currentLine) ? word : $"{currentLine} {word}";

                    if (paint.MeasureText(testLine) <= maxWidth)
                    {
                        currentLine = testLine;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(currentLine))
                        {
                            resultLines.Add(currentLine);
                        }

                        currentLine = word;
                    }
                }

                if (!string.IsNullOrWhiteSpace(currentLine))
                {
                    resultLines.Add(currentLine);
                }
            }

            return resultLines;
        }

        private void AddBrand(
            SKCanvas canvas,
            int imageWidth,
            int imageHeight,
            string siteTitle,
            SKTypeface typeface,
            float fontSize
        )
        {
            var brandFontSize = imageHeight / 20f;
            var xPadding = imageWidth / 30f;
            var footerHeight = brandFontSize * 2f;

            using var rectPaint = new SKPaint
            {
                Color = SKColor.Parse("#134e5e"),
                Style = SKPaintStyle.Fill
            };
            canvas.DrawRect(0, imageHeight - footerHeight, imageWidth, footerHeight, rectPaint);

            using var textPaint = new SKPaint
            {
                Typeface = typeface,
                TextSize = brandFontSize,
                Color = SKColor.Parse("#c44225"),
                IsAntialias = true,
                TextAlign = SKTextAlign.Right
            };

            canvas.DrawText(siteTitle, imageWidth - xPadding, imageHeight - (footerHeight - brandFontSize) / 2f, textPaint);
        }
    }
}
