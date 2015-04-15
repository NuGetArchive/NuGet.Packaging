using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuGet.RuntimeModel
{
    public class RuntimeGraph
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
    }
}
