using System.Collections.Generic;
using Statiq.Common;
using Statiq.Testing;

namespace StatiqHelpers.Unit.Tests.Modules
{
    public class ModuleTestHelpersStatic
    {
        public static TestDocument GetTestDocument( IReadOnlyCollection<KeyValuePair<string, object>> metadata)
            => new TestDocument(metadata);
    }
}
