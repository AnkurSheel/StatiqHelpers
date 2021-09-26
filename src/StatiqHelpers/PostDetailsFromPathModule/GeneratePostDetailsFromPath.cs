using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Statiq.Common;

namespace StatiqHelpers.PostDetailsFromPathModule
{
    public class GeneratePostDetailsFromPath : ParallelModule
    {
        protected override Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            context.LogDebug($"Read file {input.Source}");

            var postDetailsFromPath = input.GetPostDetailsFromPath();
            var date = $"{postDetailsFromPath["year"].Value}-{postDetailsFromPath["month"].Value}-{postDetailsFromPath["date"].Value}";

            return Task.FromResult(
                input.Clone(
                        new MetadataItems
                        {
                            { MetaDataKeys.PublishedDate, DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture) },
                            { MetaDataKeys.Slug, postDetailsFromPath["slug"].Value }
                        })
                    .Yield());
        }
    }
}
