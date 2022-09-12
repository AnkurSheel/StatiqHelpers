using Microsoft.Extensions.Logging;

namespace StatiqHelpers.Modules
{
    public class GeneratePageDetailsFromPath : ParallelModule
    {
        protected override Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            context.LogDebug($"Read file {input.Source}");

            var path = input.Source.GetRelativeInputPath().Parent;

            if (input.Source.MediaType == MediaTypes.Razor)
            {
                path = path.Combine(input.Source.FileNameWithoutExtension);
            }

            var slug = path.ToString();

            if (string.Compare(slug, "pages", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                throw new NotSupportedException("Markdown files should be in a sub folder");
            }

            slug = slug.Replace("pages/", "");

            return Task.FromResult(
                input.Clone(
                        new MetadataItems
                        {
                            { MetaDataKeys.Slug, slug }
                        })
                    .Yield());
        }
    }
}
