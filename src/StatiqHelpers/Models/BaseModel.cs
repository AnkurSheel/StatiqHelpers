using Statiq.Common;

namespace StatiqHelpers.Models
{
    public record BaseModel(IDocument Document, IExecutionContext Context);
}
