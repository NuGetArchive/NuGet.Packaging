using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Frameworks
{
    /// <summary>
    /// FrameworkExpander finds all equivalent and compatible frameworks for a NuGetFramework
    /// </summary>
    public class FrameworkExpander
    {
        private readonly IFrameworkNameProvider _mappings;

        public FrameworkExpander()
            : this(DefaultFrameworkNameProvider.Instance)
        {

        }

        public FrameworkExpander(IFrameworkNameProvider mappings)
        {
            _mappings = mappings;
        }

        /// <summary>
        /// Return all possible equivalent, subset, and known compatible frameworks.
        /// </summary>
        public IEnumerable<NuGetFramework> Expand(NuGetFramework framework)
        {
            HashSet<NuGetFramework> seen = new HashSet<NuGetFramework>(NuGetFramework.Comparer) { framework };
            Stack<NuGetFramework> toExpand = new Stack<NuGetFramework>();
            toExpand.Push(framework);

            while (toExpand.Count > 0)
            {
                var curFramework = toExpand.Pop();
                bool hasPlatform = curFramework.HasPlatform;

                List<NuGetFramework> expansions = new List<NuGetFramework>(ExpandInternal(curFramework));

                if (hasPlatform)
                {
                    // expand both with and without the platform
                    var withoutPlatform = new NuGetFramework(curFramework.Framework, curFramework.Version, curFramework.Profile);

                    if (seen.Add(withoutPlatform))
                    {
                        yield return withoutPlatform;

                        expansions.AddRange(ExpandInternal(withoutPlatform));
                    }
                }

                foreach (var expansion in expansions)
                {
                    // only return distinct frameworks
                    if (seen.Add(expansion))
                    {
                        yield return expansion;

                        toExpand.Push(expansion);
                    }

                    if (hasPlatform && !expansion.HasPlatform)
                    {
                        // add original platform to the expansion since, it would also be compatible
                        var withOriginalPlatform = new NuGetFramework(expansion.Framework,
                            expansion.Version, expansion.Profile,
                            curFramework.PlatformIdentifier, curFramework.PlatformVersion);

                        if (seen.Add(withOriginalPlatform))
                        {
                            yield return withOriginalPlatform;

                            toExpand.Push(withOriginalPlatform);
                        }
                    }
                }
            }

            yield break;
        }

        /// <summary>
        /// Finds all expansions using the mapping provider
        /// </summary>
        private IEnumerable<NuGetFramework> ExpandInternal(NuGetFramework framework)
        {
            // check the framework directly, this includes profiles which the range doesn't return
            IEnumerable<NuGetFramework> directlyEquivalent = null;
            if (_mappings.TryGetEquivalentFrameworks(framework, out directlyEquivalent))
            {
                foreach (var eqFw in directlyEquivalent)
                {
                    yield return eqFw;
                }
            }

            // 0.0 through the current framework
            FrameworkRange frameworkRange = new FrameworkRange(
                new NuGetFramework(framework.Framework, FrameworkConstants.EmptyVersion, framework.Profile, 
                    framework.PlatformIdentifier, framework.PlatformVersion),
                framework);

            IEnumerable<NuGetFramework> equivalent = null;
            if (_mappings.TryGetEquivalentFrameworks(frameworkRange, out equivalent))
            {
                foreach (var eqFw in equivalent)
                {
                    yield return eqFw;
                }
            }

            // find all possible sub set frameworks if no profile is used
            if (!framework.HasProfile)
            {
                IEnumerable<string> subSetFrameworks = null;
                if (_mappings.TryGetSubSetFrameworks(framework.Framework, out subSetFrameworks))
                {
                    foreach (var subFramework in subSetFrameworks)
                    {
                        // clone the framework but use the sub framework instead
                        yield return new NuGetFramework(subFramework, framework.Version, framework.Profile, 
                            framework.PlatformIdentifier, framework.PlatformVersion);
                    }
                }
            }

            // explicit compatiblity mappings
            IEnumerable<FrameworkRange> ranges = null;
            if (_mappings.TryGetCompatibilityMappings(framework, out ranges))
            {
                foreach (var range in ranges)
                {
                    yield return range.Min;

                    if (!range.Min.Equals(range.Max))
                    {
                        yield return range.Max;
                    }
                }
            }

            // platform variations
            if (framework.HasPlatform)
            {
                // frameworks with platforms are compatible with the same framework + the empty platform
                yield return new NuGetFramework(framework.Framework, framework.Version, framework.Profile);

                // check for platform mappings
                IEnumerable<NuGetTargetPlatform> targetPlatforms = null;
                if (_mappings.TryGetPlatformMappings(framework.Platform, out targetPlatforms))
                {
                    foreach (var platform in targetPlatforms)
                    {
                        yield return new NuGetFramework(framework.Framework,
                            framework.Version, framework.Profile, 
                            platform.Platform, platform.Version);
                    }
                }
            }

            yield break;
        }
    }
}
