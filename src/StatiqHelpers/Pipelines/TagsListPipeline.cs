using System.Linq;
using Statiq.Common;
using Statiq.Core;
using Statiq.Razor;
using StatiqHelpers.Extensions;
using StatiqHelpers.Models;

namespace StatiqHelpers.Pipelines
{
    public class TagsListPipeline : Pipeline
    {
        public TagsListPipeline()
        {
            Dependencies.Add(nameof(TagsPipeline));

            InputModules = new ModuleList
            {
                new ReadFiles("TagsList.cshtml")
            };

            ProcessModules = new ModuleList
            {
                new SetDestination("tags.html"),
                new SetMetadata("Title", "All Tags")
            };

            PostProcessModules = new ModuleList
            {
                new RenderRazor().WithModel(
                    Config.FromDocument(
                        (document, context) =>
                        {
                            var tags = context.Outputs.FromPipeline(nameof(TagsPipeline))
                                .Select(x => x.AsTag(context))
                                .OrderByDescending(x => x.Posts.Count)
                                .ThenBy(x => x.Name)
                                .ToList();
                            return new Tags(document, context, tags);
                        })),
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
