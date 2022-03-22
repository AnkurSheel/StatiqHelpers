using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Statiq.Common;
using StatiqHelpers.Extensions;

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
