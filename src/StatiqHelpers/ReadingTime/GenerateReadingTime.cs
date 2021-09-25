using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Statiq.Common;

namespace StatiqHelpers.ReadingTime
{
    public class GenerateReadingTime : ParallelModule
    {
        private readonly IReadingTimeService _readingTimeService;

        public GenerateReadingTime(IReadingTimeService readingTimeService)
        {
            _readingTimeService = readingTimeService;
        }

        protected override async Task<IEnumerable<IDocument>> ExecuteInputAsync(IDocument input, IExecutionContext context)
        {
            context.LogDebug($"Read file {input.Source}");

            using var textReader = input.GetContentTextReader();
            var content = await textReader.ReadToEndAsync();

            return input.Clone(
                    new MetadataItems
                    {
                        { Keys.ReadingTime, _readingTimeService.GetReadingTime(content, context.GetInt("ReadingTimeWordsPerMinute", 200)) }
                    })
                .Yield();
        }
    }
}
