using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuGet.RuntimeModel
{
    public class RuntimeGraph : IEquatable<RuntimeGraph>
    {
        public IDictionary<string, RuntimeDescription> Runtimes { get; }

        public RuntimeGraph()
        {
            Runtimes = new Dictionary<string, RuntimeDescription>();
        }

        public RuntimeGraph(IDictionary<string, RuntimeDescription> runtimes)
        {
            Runtimes = new Dictionary<string, RuntimeDescription>(runtimes);
        }

        public bool Equals(RuntimeGraph other) => other != null && other.Runtimes
            .OrderBy(pair => pair.Key)
            .SequenceEqual(other.Runtimes.OrderBy(pair => pair.Key));

        public override bool Equals(object obj) => Equals(obj as RuntimeGraph);
        public override int GetHashCode() => Runtimes.GetHashCode();
    }
}
