using Microsoft.Extensions.Logging;
using Statiq.Web.Pipelines;

namespace StatiqHelpers.Analyzers;

public class TitleLengthAnalyzer : Analyzer
{
    public TitleLengthAnalyzer()
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

        var title = document.GetTitle();

        if (title.Length > 60)
        {
            context.AddAnalyzerResult(document, $"Max Title Length : 60: Reduce title length by {title.Length - 60} characters : '{title}'");
        }
        // else if (title.Length < 30)
        // {
        //     context.AddAnalyzerResult(document, $"Title length ({title.Length}) is less than 30 characters : '{title}'");
        // }

        return Task.CompletedTask;
    }
}
