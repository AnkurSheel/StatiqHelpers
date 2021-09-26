using Statiq.Common;

namespace StatiqHelpers.Models
{
    public record BaseModel
    {
        public IDocument Document { get; }

        public IExecutionContext Context { get; }

        public BaseModel(IDocument document, IExecutionContext context)
        {
            Document = document;
            Context = context;
        }
    }
}
