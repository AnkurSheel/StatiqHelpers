using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Statiq.Common;

namespace StatiqHelpers.PostDetailsFromPathModule
{
    public class GeneratePageDetailsFromPath : ParallelModule
    {
        protected override Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            context.LogDebug($"Read file {input.Source}");

            var slug = input.Source.GetRelativeInputPath().Parent.Combine(input.Source.FileNameWithoutExtension).ToString();
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
