using StatiqHelpers.CustomExtensions;

namespace StatiqHelpers.Modules
{
    public class OptimizeSlug : ParallelModule
    {

        protected override Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            var path = input.GetPath(MetaDataKeys.Slug);

            if (path.IsNullOrEmpty)
            {
                throw new ArgumentNullException(MetaDataKeys.Slug);
            }

            path = path.OptimizeSlug();

            return Task.FromResult(
                input.Clone(
                        new MetadataItems
                        {
                            { MetaDataKeys.Slug, path }
                        })
                    .Yield());
        }
    }
}
