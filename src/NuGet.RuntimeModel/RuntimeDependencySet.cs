using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuGet.RuntimeModel
{
    public class RuntimeDependencySet : IEquatable<RuntimeDependencySet>
    {
        public string Id { get; }
        public IList<RuntimePackageDependency> Dependencies { get; }

        public RuntimeDependencySet(string id) : this(id, Enumerable.Empty<RuntimePackageDependency>()) { }
        public RuntimeDependencySet(string id, IEnumerable<RuntimePackageDependency> dependencies)
        {
            Id = id;
            Dependencies = dependencies.ToList();
        }

        public bool Equals(RuntimeDependencySet other) => other != null &&
            string.Equals(other.Id, Id, StringComparison.Ordinal) &&
            Dependencies.OrderBy(d => new { d.Id, d.Version }).SequenceEqual(other.Dependencies.OrderBy(d => new { d.Id, d.Version }));
        public override bool Equals(object obj) => Equals(obj as RuntimeDependencySet);
        public override int GetHashCode() => HashCodeCombiner.Start()
            .AddObject(Id)
            .AddObject(Dependencies);
    }
}
