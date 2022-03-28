using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Statiq.Common;
using StatiqHelpers.Extensions;
using StatiqHelpers.ImageHelpers;

namespace StatiqHelpers.Modules
{
    public class GenerateSocialImages : ParallelModule
    {
        private readonly IImageService _imageService;

        public GenerateSocialImages(IImageService imageService)
        {
            _imageService = imageService;
        }

        protected override async Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            context.LogDebug($"Read file {input.Source}");

            var readingTimeData = input.GetReadingTime();
            var centerText = $"{input.GetTitle().ToUpper()}{Environment.NewLine}";

            centerText += readingTimeData.RoundedMinutes < 1
                ? $"{readingTimeData.Seconds} sec"
                : $"{readingTimeData.RoundedMinutes} min";

            var coverImagePath = input.GetCoverImagePath();

            //Todo: Check if this can be replaced with GetCoverImageLink
            if (coverImagePath != null)
            {
                coverImagePath = coverImagePath.StartsWith(Constants.ImagesDirectory)
                    ? $"{context.FileSystem.GetRootPath()}/input/{coverImagePath}"
                    : $"{input.Source.Parent.FullPath}/{coverImagePath}";
            }

            var stream = await _imageService.CreateImageDocument(1200, 630, coverImagePath, context.GetSiteTitle(), centerText);

            var facebookDoc = context.CreateDocument(
                input.Source,
                $"./{Constants.SocialImagesDirectory}/{input.Destination.FileNameWithoutExtension}-facebook.png",
                context.GetContentProvider(stream));

            context.LogDebug($"Created {facebookDoc.Destination}");

            stream = await _imageService.CreateImageDocument(440, 220, coverImagePath, context.GetSiteTitle(), centerText);

            var twitterDoc = context.CreateDocument(
                input.Source,
                $"./{Constants.SocialImagesDirectory}/{input.Destination.FileNameWithoutExtension}-twitter.png",
                context.GetContentProvider(stream));

            context.LogDebug($"Created {twitterDoc.Destination}");

            return new[]
            {
                facebookDoc,
                twitterDoc
            };
        }
    }
}
