using ByteSizeLib;
using Microsoft.Extensions.Logging;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
            string fontPath)
        {
            Image template = new Image<Rgb24>(width, height);
            Image? coverImage = null;

            var font = _fontHelper.InstallFont(fontPath);
            var fontSize = height / 10;

            if (coverImagePath != null)
            {
                coverImage = await Image.LoadAsync(coverImagePath);
            }

            template.Mutate(
                imageContext =>
                {
                    AddGradient(width, height, imageContext);

                    if (coverImage != null)
                    {
                        using var thumbnail = AddThumbnail(width, height, coverImage);
                        imageContext.DrawImage(thumbnail, new Point(0, 0), 1f);
                    }

                    AddCenterText(imageContext, width, height, centerText, new Font(font, fontSize, FontStyle.Bold));
                    AddBrand(imageContext, width, height, siteTitle, new Font(font, fontSize, FontStyle.Regular));
                });

            Stream output = new MemoryStream();

            await template.SaveAsPngAsync(output);

            return output;
        }

        private Image AddThumbnail(int width, int height, Image thumbnail)
        {
            thumbnail.Mutate(
                imageContext =>
                {
                    imageContext.SetGraphicsOptions(
                        new GraphicsOptions
                        {
                            Antialias = true
                        });

                    imageContext.Resize(
                        new ResizeOptions
                        {
                            Position = AnchorPositionMode.Center,
                            Size = new Size(width, height),
                            Mode = ResizeMode.Pad,
                        });
                    DarkenImage(imageContext);
                });
            return thumbnail;
        }

        public async Task ResizeImages(IReadOnlyList<string> imagePaths, int newWidth, int newHeight)
        {
            var totalPre = 0L;
            var totalPost = 0L;

            foreach (var imagePath in imagePaths)
            {
                var fileInfo = new FileInfo(imagePath);
                var preSize = 0L;
                var image = await Image.LoadAsync(imagePath);

                var originalSize = image.Size();

                if ((originalSize.Width == newWidth && (newHeight == 0 || originalSize.Height == newHeight))
                    || (originalSize.Height == newHeight && (newWidth == 0 || originalSize.Width == newWidth)))
                {
                    // _logger.Log(
                    //     LogLevel.Information,
                    //     "No change in size for {Path}. Ignoring",
                    //     imagePath);

                    continue;
                }

                preSize = fileInfo.Length;

                totalPre += preSize;

                var resizeOptions = new ResizeOptions
                {
                    Size = new Size(newWidth, newHeight),
                    Compand = true
                };

                image.Mutate(imageContext => imageContext.Resize(resizeOptions));

                switch (fileInfo.Extension)
                {
                    case ".jpg":
                    case ".jpeg":
                        await image.SaveAsJpegAsync(
                            imagePath,
                            new JpegEncoder
                            {
                                Quality = 80,
                                Subsample = JpegSubsample.Ratio444
                            });
                        break;
                    case ".png":
                        await image.SaveAsPngAsync(
                            imagePath,
                            new PngEncoder
                            {
                                CompressionLevel = PngCompressionLevel.BestCompression
                            });
                        break;
                    default:
                        _logger.Log(LogLevel.Error, "No support for extension {Extension} in {Path}", fileInfo.Extension, imagePath);
                        break;
                }

                fileInfo = new FileInfo(imagePath);
                var postSize = fileInfo.Length;
                totalPost += postSize;

                var percentChanged = Math.Abs(postSize - preSize) / (decimal)preSize;

                _logger.Log(
                    LogLevel.Information,
                    "Resized {Path} with {Size}. Changed from {PreSize} to {PostSize} ({PercentChanged:P})",
                    imagePath,
                    originalSize,
                    ByteSize.FromBytes(preSize).ToString(),
                    ByteSize.FromBytes(postSize).ToString(),
                    percentChanged);
            }

            _logger.Log(
                LogLevel.Information,
                "Resizing complete. Updated Size from {PreSize} to {PostSize}",
                ByteSize.FromBytes(totalPre).ToString(),
                ByteSize.FromBytes(totalPost).ToString());
        }

        private void AddGradient(int width, int height, IImageProcessingContext imageContext)
        {
            imageContext.Fill(
                new LinearGradientBrush(
                    new PointF(0, 0),
                    new PointF(width / 2, height / 2),
                    GradientRepetitionMode.Reflect,
                    new ColorStop(0f, Color.ParseHex("#16222A")),
                    new ColorStop(0.5f, Color.ParseHex("#3A6073"))));
        }

        private void DarkenImage(IImageProcessingContext imageContext)
        {
            imageContext.Brightness(0.5f);
        }

        private void AddCenterText(
            IImageProcessingContext imageContext,
            int imageWidth,
            int imageHeight,
            string centerText,
            Font font)
        {

            var xPadding = imageWidth / 30;
            var drawingOptions = new DrawingOptions
            {
                GraphicsOptions = new GraphicsOptions
                {
                    Antialias = true
                },
                TextOptions = new TextOptions
                {
                    WrapTextWidth = imageWidth - xPadding * 2,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                }
            };

            var verticalCenter = (imageHeight - font.Size) / 2;
            imageContext.DrawText(drawingOptions, centerText, font, Color.MediumPurple, new PointF(xPadding + 2, verticalCenter + 2));
            imageContext.DrawText(drawingOptions, centerText, font, Color.White, new PointF(xPadding, verticalCenter));
        }

        private void AddBrand(
            IImageProcessingContext imageProcessingContext,
            int imageWidth,
            int imageHeight,
            string siteTitle,
            Font font)
        {
            var drawingOptions = new DrawingOptions
            {
                GraphicsOptions = new GraphicsOptions
                {
                    Antialias = true
                },
                TextOptions = new TextOptions
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                }
            };

            var fontSize = imageHeight / 20;
            var xPadding = imageWidth / 30;

            var height = fontSize * 2;
            var rectangularPolygon = new RectangularPolygon(0, imageHeight - height, imageWidth, height);
            imageProcessingContext.Fill(Color.ParseHex("#134e5e"), rectangularPolygon);
            imageProcessingContext.DrawText(
                drawingOptions,
                siteTitle,
                font,
                Color.ParseHex("#c44225"),
                new PointF(imageWidth - xPadding, imageHeight - fontSize));
        }
    }
}
