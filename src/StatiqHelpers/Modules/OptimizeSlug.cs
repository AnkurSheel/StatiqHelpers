using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Statiq.Common;

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

            var optimizedSegments = path.Segments.Select(segment => NormalizedPath.OptimizeFileName(segment.ToString())).ToList();

            path = string.Join("/", optimizedSegments);

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
