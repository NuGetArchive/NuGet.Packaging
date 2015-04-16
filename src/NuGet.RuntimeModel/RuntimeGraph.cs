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

        public RuntimeGraph(IEnumerable<RuntimeDescription> runtimes)
        {
            Runtimes = runtimes.ToDictionary(r => r.RuntimeIdentifier);
        }

        /// <summary>
        /// Merges the content of the other runtime graph in to this runtime graph
        /// </summary>
        /// <param name="other">The other graph to merge in to this graph</param>
        public void MergeIn(RuntimeGraph other)
        {
            foreach(var otherRuntime in other.Runtimes.Values)
            {
                // Check if we already have the runtime defined
                RuntimeDescription myRuntime;
                if(Runtimes.TryGetValue(otherRuntime.RuntimeIdentifier, out myRuntime))
                {
                    myRuntime.MergeIn(otherRuntime);
                }
                else
                {
                    Runtimes.Add(otherRuntime.RuntimeIdentifier, otherRuntime);
                }
            }
        }

        public bool Equals(RuntimeGraph other) => other != null && other.Runtimes
            .OrderBy(pair => pair.Key)
            .SequenceEqual(other.Runtimes.OrderBy(pair => pair.Key));

        public override bool Equals(object obj) => Equals(obj as RuntimeGraph);
        public override int GetHashCode() => Runtimes.GetHashCode();
    }
}
