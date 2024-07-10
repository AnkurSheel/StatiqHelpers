using Statiq.Razor;
using StatiqHelpers.CustomExtensions;
using StatiqHelpers.Models;

namespace StatiqHelpers.Pipelines;

public class TagsListPipeline : Pipeline
{
    public TagsListPipeline(PipelineOptions options)
    {
        Dependencies.Add(nameof(TagsPipeline));

        InputModules = new ModuleList
        {
            new ReadFiles("TagsList.cshtml")
        };

        ProcessModules = new ModuleList
        {
            new SetDestination("tags.html"),
            new SetMetadata(Keys.Title, "All Tags")
        };

        PostProcessModules = new ModuleList
        {
            new RenderRazor().WithModel(Config.FromDocument((document, context) =>
            {
                var tags = context.Outputs.FromPipeline(nameof(TagsPipeline))
                    .Select(x => x.AsTag(options, context))
                    .OrderByDescending(x => x.Posts.Count)
                    .ThenBy(x => x.Name)
                    .ToList();
                return new Tags(document, context, tags);
            }))
        };

        OutputModules = new ModuleList
        {
            new WriteFiles()
        };
    }
}
