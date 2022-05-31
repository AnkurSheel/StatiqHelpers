using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using StatiqHelpers.Extensions;

namespace StatiqHelpers.Pipelines
{
    public class TagsPipeline : Pipeline
    {
        public TagsPipeline()
        {
            Dependencies.Add(nameof(PostPipeline));

            InputModules = new ModuleList
            {
                new ReadFiles("Tags.cshtml")
            };

            ProcessModules = new ModuleList
            {
                new MergeDocuments
                {
                    new ReplaceDocuments(nameof(PostPipeline)),
                    new GroupDocuments("tags")
                }.Reverse(),
                new SetMetadata(Keys.Title, Config.FromDocument((document, context) => $"Posts tagged as \"{document.GetString(Keys.GroupKey)}\"")),
                new SetMetadata(MetaDataKeys.TagName, Config.FromDocument((document, context) => document.GetString(Keys.GroupKey))),
                new SetDestination(
                    Config.FromDocument((doc, ctx) => new NormalizedPath("tags").Combine($"{doc.GetString(Keys.GroupKey)}.html").OptimizeSlug())),
            };

            PostProcessModules = new ModuleList
            {
                new RenderRazor().WithModel(Config.FromDocument((document, context) => document.AsTag(context)))
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
