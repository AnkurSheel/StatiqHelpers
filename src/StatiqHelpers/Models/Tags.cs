using Statiq.Common;

namespace StatiqHelpers.Models
{
    public record Tags : BaseModel
    {
        public Tags(IDocument document, IExecutionContext context, IReadOnlyList<Tag> tags) : base(document, context)
        {
            AllTags = tags;
        }

        public IReadOnlyList<Tag> AllTags { get; }
    }
}
