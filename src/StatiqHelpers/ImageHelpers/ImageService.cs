using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ByteSizeLib;
using Microsoft.Extensions.Logging;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace StatiqHelpers.ImageHelpers
{
    public class ImageService : IImageService
    {
        private readonly FontFamily _cookieFont;
        private readonly ILogger<ImageService> _logger;

        public ImageService(ILogger<ImageService> logger)
        {
            _logger = logger;
            _cookieFont = new FontCollection().Install("./input/assets/fonts/Cookie-Regular.ttf");
        }

        public async Task<Stream> CreateImageDocument(
            int width,
            int height,
            string? coverImagePath,
            string siteTitle,
            string centerText)
        {
            Image template = new Image<Rgb24>(width, height);

            template.Mutate(
                async imageContext =>
                {
                    AddGradient(width, height, imageContext);

                    if (coverImagePath != null)
                    {
                        using var thumbnail = await AddThumbnail(width, height, coverImagePath);
                        imageContext.DrawImage(thumbnail, new Point(0, 0), 1f);
                    }

                    AddCenterText(
                        imageContext,
                        width,
                        height,
                        centerText);
                    AddBrand(
                        imageContext,
                        width,
                        height,
                        siteTitle);
                });

            Stream output = new MemoryStream();

            await template.SaveAsPngAsync(output);

            return output;
        }

        private async Task<Image> AddThumbnail(int width, int height, string coverImagePath)
        {
            Image thumbnail = await Image.LoadAsync(coverImagePath);

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
                var preSize = new FileInfo(imagePath).Length;
                totalPre += preSize;

                var image = await Image.LoadAsync(imagePath);

                var originalSize = image.Size();
                var resizeOptions = new ResizeOptions
                {
                    Size = new Size(newWidth, newHeight),
                    Compand = true
                };

                image.Mutate(imageContext => imageContext.Resize(resizeOptions));

                await image.SaveAsJpegAsync(imagePath);
                var postSize = new FileInfo(imagePath).Length;
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
            string centerText)
        {
            var fontSize = imageHeight / 10;
            var titleFont = new Font(_cookieFont, fontSize, FontStyle.Bold);

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

            var verticalCenter = (imageHeight - titleFont.Size) / 2;
            imageContext.DrawText(
                drawingOptions,
                centerText,
                titleFont,
                Color.MediumPurple,
                new PointF(xPadding + 2, verticalCenter + 2));
            imageContext.DrawText(
                drawingOptions,
                centerText,
                titleFont,
                Color.White,
                new PointF(xPadding, verticalCenter));
        }

        private void AddBrand(
            IImageProcessingContext imageProcessingContext,
            int imageWidth,
            int imageHeight,
            string siteTitle)
        {
            DrawingOptions drawingOptions = new DrawingOptions
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
            var font = new Font(_cookieFont, fontSize, FontStyle.Regular);

            var height = fontSize * 2;
            var rectangularPolygon = new RectangularPolygon(
                0,
                imageHeight - height,
                imageWidth,
                height);
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
