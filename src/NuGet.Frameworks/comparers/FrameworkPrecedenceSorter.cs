using System;
using System.Collections.Generic;

namespace NuGet.Frameworks
{
    /// <summary>
    /// Sorts frameworks according to the framework mappings
    /// </summary>
    public class FrameworkPrecedenceSorter : IComparer<NuGetFramework>
    {
        private readonly IFrameworkNameProvider _mappings;

        public FrameworkPrecedenceSorter(IFrameworkNameProvider mappings)
        {
            _mappings = mappings;
        }

        public int Compare(NuGetFramework x, NuGetFramework y)
        {
            // This is a simple wrapper for the compare method on the name provider.
            return _mappings.CompareFrameworks(x, y);
        }
    }
}