using Microsoft.Extensions.DependencyInjection;
using Statiq.Web.Pipelines;
using StatiqHelpers.ImageHelpers;
using StatiqHelpers.Modules.ReadingTime;
using StatiqHelpers.Modules.RelatedPosts;
using StatiqHelpers.Pipelines;

namespace StatiqHelpers.CustomExtensions;

public static class BootstrapperExtensions
{
    public static Bootstrapper AddPipelines(this Bootstrapper bootstrapper)
    {
        bootstrapper.AddPipeline<PostPipeline>()
            .AddPipeline<PagesPipeline>()
            .AddPipeline<ImagesPipeline>()
            .AddPipeline<CssPipeline>()
            .AddPipeline<FontsPipeline>()
            .AddPipeline<DownloadsPipeline>()
            .AddPipeline<ScriptsPipeline>()
            .AddPipeline<RssPipeline>()
            .AddPipeline<HomePipeline>()
            .AddPipeline<SocialImagesPipeline>()
            .AddPipeline<PostListPipeline>()
            .AddPipeline<TagsListPipeline>()
            .AddPipeline<TagsPipeline>()
            .AddPipeline<SitemapPipeline>()
            .AddPipeline(nameof(AnalyzeContent), new Pipeline
            {
                Deployment = true,
                ExecutionPolicy = ExecutionPolicy.Normal,
                InputModules =
                {
                    new ReplaceDocuments(nameof(ScriptsPipeline), nameof(HomePipeline), nameof(PagesPipeline),
                        nameof(PostListPipeline), nameof(PostPipeline), nameof(TagsListPipeline), nameof(TagsPipeline))
                }
            });
        return bootstrapper;
    }

    public static Bootstrapper AddServices(this Bootstrapper bootstrapper)
    {
        bootstrapper.ConfigureServices(services =>
        {
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IReadingTimeService, ReadingTimeService>();
            services.AddTransient<IRelatedPostsService, RelatedPostsService>();
            services.AddTransient<IRelatedPostsPointCalculator, RelatedPostsPointCalculator>();
            services.AddSingleton(new PipelineOptions(documentList =>
                documentList.OrderBy(document => document.GetTitle()).ToDocumentList()));
        });
        return bootstrapper;
    }

    public static Bootstrapper RemovePipelines(this Bootstrapper bootstrapper)
    {
        bootstrapper.ConfigureEngine(engine =>
        {
            engine.Pipelines.Remove(nameof(Inputs));
            engine.Pipelines.Remove(nameof(Assets));
            engine.Pipelines.Remove(nameof(Content));
            engine.Pipelines.Remove(nameof(Sitemap));
            engine.Pipelines.Remove(nameof(Archives));
            engine.Pipelines.Remove(nameof(Feeds));
            engine.Pipelines.Remove(nameof(Data));
            engine.Pipelines.Remove(nameof(Redirects));
            engine.Pipelines.Remove(nameof(SearchIndex));
            engine.Pipelines.Remove(nameof(AnalyzeContent));
        });

        return bootstrapper;
    }
}