using Statiq.Common;
using Statiq.Core;
using StatiqHelpers.ImageHelpers;
using StatiqHelpers.Modules;

namespace StatiqHelpers.Pipelines
{
    public class SocialImagesPipeline : Pipeline
    {
        public SocialImagesPipeline(IImageService imageService)
        {
            Dependencies.AddRange(nameof(PostPipeline));

            ProcessModules = new ModuleList
            {
                new ConcatDocuments(Dependencies.ToArray()),
                new CacheDocuments(new GenerateSocialImages(imageService))
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
