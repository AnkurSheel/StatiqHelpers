﻿namespace StatiqHelpers.Pipelines
{
    public record PostListOptions(Func<IDocument, object> OrderFunction, bool Descending = false);
}
