using Statiq.Common;
using Statiq.Core;
using Statiq.Markdown;
using Statiq.Razor;
using Statiq.Yaml;
using StatiqHelpers.Extensions;
using StatiqHelpers.PostDetailsFromPathModule;
using StatiqHelpers.ReadingTimeModule;
using StatiqHelpers.Rss;

namespace StatiqHelpers.Pipelines
{
    public class PostPipeline : Pipeline
    {
        public PostPipeline(IReadingTimeService readingTimeService)
        {
            InputModules = new ModuleList
            {
                new ReadFiles("posts/**/*.{md,mdx}")
            };

            ProcessModules = new ModuleList
            {
                new CacheDocuments
                {
                    new ExtractFrontMatter(new ParseYaml()),
                    new GeneratePostDetailsFromPath(),
                    new GenerateRssMetaData(),
                    new ReplaceInContent(
                    @"!\[(?<alt>.*)\]\(./images/(?<imagePath>.*)\)",
                    Config.FromDocument((document, context) => $"![$1](./$2)"))
                    .IsRegex(),
                    new ReplaceInContent(
                            @"!\[(?<alt>.*)\]\(./(?<imagePath>.*)\)",
                            Config.FromDocument((document, context) => $"![$1](/{Constants.PostImagesDirectory}/{document.GetString(MetaDataKeys.Slug)}/$2)"))
                        .IsRegex(),
                    new GenerateReadingTime(readingTimeService),
                    new RenderMarkdown().UseExtensions(),
                    new OptimizeFileName(MetaDataKeys.Slug),
                    new SetDestination(Config.FromDocument((doc, ctx) => new NormalizedPath(Constants.BlogPath).Combine($"{doc.GetString(MetaDataKeys.Slug)}.html"))),
                }
            };

            PostProcessModules = new ModuleList
            {
                new RenderRazor().WithBaseModel()
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
