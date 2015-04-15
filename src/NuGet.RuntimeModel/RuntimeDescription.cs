using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuGet.RuntimeModel
{
    public class RuntimeDescription
    {
        public string RuntimeIdentifier { get; }
        public IReadOnlyList<string> InheritedRuntimes { get; }
        public IList<RuntimeDependencySet> AdditionalDependencies { get; }

        public RuntimeDescription(string runtimeIdentifier, IEnumerable<string> inheritedRuntimes) : this(runtimeIdentifier, inheritedRuntimes, Enumerable.Empty<RuntimeDependencySet>()) { }

        public RuntimeDescription(string runtimeIdentifier, IEnumerable<string> inheritedRuntimes, IEnumerable<RuntimeDependencySet> additionalDependencies)
        {
            RuntimeIdentifier = runtimeIdentifier;
            InheritedRuntimes = inheritedRuntimes.ToList().AsReadOnly();
            AdditionalDependencies = additionalDependencies.ToList(); 
        }
    }
}
