using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuGet.RuntimeModel
{
    public class RuntimeDescription : IEquatable<RuntimeDescription>
    {
        public string RuntimeIdentifier { get; }
        public IReadOnlyList<string> InheritedRuntimes { get; }
        public IList<RuntimeDependencySet> AdditionalDependencies { get; }

        public RuntimeDescription(string runtimeIdentifier) : this(runtimeIdentifier, Enumerable.Empty<string>(), Enumerable.Empty<RuntimeDependencySet>()) { }
        public RuntimeDescription(string runtimeIdentifier, IEnumerable<string> inheritedRuntimes) : this(runtimeIdentifier, inheritedRuntimes, Enumerable.Empty<RuntimeDependencySet>()) { }

        public RuntimeDescription(string runtimeIdentifier, IEnumerable<string> inheritedRuntimes, IEnumerable<RuntimeDependencySet> additionalDependencies)
        {
            RuntimeIdentifier = runtimeIdentifier;
            InheritedRuntimes = inheritedRuntimes.ToList().AsReadOnly();
            AdditionalDependencies = additionalDependencies.ToList();
        }

        public bool Equals(RuntimeDescription other) => other != null &&
            string.Equals(other.RuntimeIdentifier, RuntimeIdentifier, StringComparison.Ordinal) &&
            InheritedRuntimes.OrderBy(s => s).SequenceEqual(other.InheritedRuntimes.OrderBy(s => s)) &&
            AdditionalDependencies.OrderBy(d => d.Id).SequenceEqual(other.AdditionalDependencies.OrderBy(d => d.Id));
        public override bool Equals(object obj) => Equals(obj as RuntimeDescription);
        public override int GetHashCode() => HashCodeCombiner.Start()
            .AddObject(RuntimeIdentifier)
            .AddObject(InheritedRuntimes)
            .AddObject(AdditionalDependencies);
    }
}
