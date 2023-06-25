using Microsoft.Extensions.Logging;
using Statiq.Web.Pipelines;
using StatiqHelpers.CustomExtensions;

namespace StatiqHelpers.Analyzers;

public class ExcerptLengthAnalyzer : Analyzer
{
    public ExcerptLengthAnalyzer()
    {
        PipelinePhases.Add(nameof(AnalyzeContent), Phase.Process);
    }

    public override LogLevel LogLevel { get; set; } = LogLevel.Warning;

    protected override Task AnalyzeDocumentAsync(IDocument document, IAnalyzerContext context)
    {
        if (!document.MediaTypeEquals(MediaTypes.Html))
        {
            return Task.CompletedTask;
        }

        var excerpt = document.GetExcerpt();

        if (excerpt== null)
        {
            // context.AddAnalyzerResult(document, $"Missing excerpt");
            return Task.CompletedTask;
        }
        if (excerpt.Length > 155)
        {
            context.AddAnalyzerResult(document, $"Max Excerpt Length : 155: Reduce excerpt length by {excerpt.Length - 155} characters : '{excerpt}'");
        }

        return Task.CompletedTask;
    }
}
