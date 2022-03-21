using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Statiq.Common;
using StatiqHelpers.Extensions;

namespace StatiqHelpers.Modules
{
    public class GeneratePostDetailsFromPath : ParallelModule
    {
        private const string Year = "year";
        private const string Month = "month";
        private const string Date = "date";
        private const string Slug = "slug";

        protected override Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            context.LogDebug($"Read file {input.Source}");

            var postDetailsFromPath = input.GetPostDetailsFromPath();

            var date = $"{postDetailsFromPath[Year].Value}-{postDetailsFromPath[Month].Value}-{postDetailsFromPath[Date].Value}";

            return Task.FromResult(
                input.Clone(
                        new MetadataItems
                        {
                            { MetaDataKeys.PublishedDate, DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture) },
                            { MetaDataKeys.Slug, postDetailsFromPath[Slug].Value }
                        })
                    .Yield());
        }
    }
}
