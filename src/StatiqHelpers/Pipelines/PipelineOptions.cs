namespace StatiqHelpers.Pipelines
{
    public record PipelineOptions(Func<DocumentList<IDocument>, DocumentList<IDocument>> OrderFunction);
}
