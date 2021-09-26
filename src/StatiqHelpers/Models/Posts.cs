using System.Collections.Generic;
using Statiq.Common;

namespace StatiqHelpers.Models
{
    public record Posts : BaseModel
    {
        public Posts(IReadOnlyList<BaseModel> posts, IDocument document, IExecutionContext context) : base(document, context)
        {
            AllPosts = posts;
        }

        public IReadOnlyList<BaseModel> AllPosts { get; }
    }
}
