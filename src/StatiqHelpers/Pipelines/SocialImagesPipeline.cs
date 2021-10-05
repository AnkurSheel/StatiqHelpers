using System.Linq;
using Statiq.Common;
using Statiq.Core;
using StatiqHelpers.ImageHelpers;
using StatiqHelpers.SocialImages;

namespace StatiqHelpers.Pipelines
{
    public class SocialImagesPipeline : Pipeline
    {
        private readonly IImageService _imageService;

        public SocialImagesPipeline(IImageService imageService)
        {
            _imageService = imageService;
            Dependencies.AddRange(nameof(PostPipeline));

            ProcessModules = new ModuleList
            {
                new ConcatDocuments(Dependencies.ToArray()),
                new CacheDocuments(new GenerateSocialImages(_imageService))
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
