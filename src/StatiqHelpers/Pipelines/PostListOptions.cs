using System;
using Statiq.Common;

namespace StatiqHelpers.Pipelines
{
    public record PostListOptions(Func<IDocument, object> OrderFunction, string Title = "All Articles", bool Descending = false);
}
