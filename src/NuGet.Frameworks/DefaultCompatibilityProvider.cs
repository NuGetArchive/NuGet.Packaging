using NuGet.RuntimeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Frameworks
{
    public sealed class DefaultCompatibilityProvider : CompatibilityProvider
    {
        private DefaultCompatibilityProvider()
            : this(new RuntimeGraph())
        {

        }

        public DefaultCompatibilityProvider(RuntimeGraph runtimeGraph)
            : base(DefaultFrameworkNameProvider.Instance, runtimeGraph)
        {

        }

        private static IFrameworkCompatibilityProvider _instance;
        public static IFrameworkCompatibilityProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DefaultCompatibilityProvider();
                }

                return _instance;
            }
        }

        public static IFrameworkCompatibilityProvider WithRuntimeGraph(RuntimeGraph runtimeGraph)
        {
            return new DefaultCompatibilityProvider(runtimeGraph);
        }
    }
}
