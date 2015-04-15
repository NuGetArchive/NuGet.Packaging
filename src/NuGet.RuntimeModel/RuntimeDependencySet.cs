using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuGet.RuntimeModel
{
    public class RuntimeDependencySet
    {
        public string Id { get; }
        public IList<RuntimePackageDependency> Dependencies { get; }

        public RuntimeDependencySet(string id)
        {
            Id = id;
            Dependencies = new List<RuntimePackageDependency>();
        }
    }
}
