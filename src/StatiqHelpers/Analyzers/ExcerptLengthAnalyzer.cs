using Microsoft.Extensions.Logging;
using Statiq.Web.Pipelines;
using StatiqHelpers.CustomExtensions;

namespace StatiqHelpers.Analyzers;

public class ExcerptLengthAnalyzer : Analyzer
{
    private const int MaxLength = 160;

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

        if (excerpt?.Length > MaxLength)
        {
            context.AddAnalyzerResult(document,
                $"Max Excerpt Length : {MaxLength}: Reduce excerpt length by {excerpt.Length - MaxLength} characters : '{excerpt}'");
        }

        return Task.CompletedTask;
    }
}
