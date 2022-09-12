namespace StatiqHelpers.Models
{
    public record PageModel : BaseModel
    {
        public PageModel(IDocument document, IExecutionContext context, IReadOnlyList<IDocument> posts) : base(document, context)
        {
            Posts = posts;
        }

        public IReadOnlyList<IDocument> Posts { get; }
    }
}
