using Statiq.Highlight;
using Statiq.Markdown;
using Statiq.Razor;
using Statiq.Yaml;
using StatiqHelpers.CustomExtensions;
using StatiqHelpers.Modules;
using StatiqHelpers.Modules.ReadingTime;
using StatiqHelpers.Modules.RelatedPosts;

namespace StatiqHelpers.Pipelines;

public class PostPipeline : Pipeline
{
    public PostPipeline(IReadingTimeService readingTimeService, IRelatedPostsService relatedPostsService)
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
                new FilterDocuments(Config.FromDocument((document, context) => context.IsDevelopment() || document.GetPublishedDate() <= DateTime.UtcNow.Date)),
                new GenerateRssMetaData(),
                new ReplaceImageLinks(Constants.PostImagesDirectory),
                new GenerateReadingTime(readingTimeService),
                new ProcessShortcodes(),
                new RenderMarkdown().UseExtensions(),
                new OptimizeSlug(),
                new SetDestination(
                    Config.FromDocument((doc, ctx)
                        => new NormalizedPath(Constants.BlogPath).Combine($"{doc.GetSlug()}.html"))),
            }
        };

        PostProcessModules = new ModuleList
        {
            new GenerateRelatedPosts(relatedPostsService),
            new RenderRazor().WithBaseModel(),
            new HighlightCode().WithAutoHighlightUnspecifiedLanguage(true)
        };

        OutputModules = new ModuleList
        {
            new WriteFiles()
        };
    }
}
