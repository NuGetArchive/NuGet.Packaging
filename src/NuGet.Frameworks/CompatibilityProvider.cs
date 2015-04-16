using NuGet.RuntimeModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HashCombiner = NuGet.Frameworks.HashCodeCombiner;

namespace NuGet.Frameworks
{
    public class CompatibilityProvider : IFrameworkCompatibilityProvider
    {
        private readonly IFrameworkNameProvider _mappings;
        private readonly FrameworkExpander _expander;
        private static readonly NuGetFrameworkFullComparer _fullComparer = new NuGetFrameworkFullComparer();
        private readonly ConcurrentDictionary<Tuple<NuGetFramework, NuGetFramework>, bool> _cache;
        private readonly RuntimeGraph _runtimeGraph;

        public CompatibilityProvider(IFrameworkNameProvider mappings, RuntimeGraph runtimeGraph)
        {
            _mappings = mappings;
            _expander = new FrameworkExpander(mappings);
            _cache = new ConcurrentDictionary<Tuple<NuGetFramework, NuGetFramework>, bool>();
            _runtimeGraph = runtimeGraph;
        }

        /// <summary>
        /// Check if the frameworks are compatible.
        /// </summary>
        /// <param name="framework">Project framework</param>
        /// <param name="other">Other framework to check against the project framework</param>
        /// <returns>True if framework supports other</returns>
        public virtual bool IsCompatible(NuGetFramework framework, NuGetFramework other)
        {
            if (framework == null)
            {
                throw new ArgumentNullException("framework");
            }

            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            // check the cache for a solution
            bool? result = _cache.GetOrAdd(Tuple.Create(framework, other), (key) =>
            {
                return IsCompatibleCore(framework, other);
            });

            return result == true;
        }

        /// <summary>
        /// Actual compatibility check without caching
        /// </summary>
        protected virtual bool IsCompatibleCore(NuGetFramework framework, NuGetFramework other)
        {
            bool? result = null;

            // check if they are the exact same
            if (_fullComparer.Equals(framework, other))
            {
                return true;
            }

            // special cased frameworks
            if (!framework.IsSpecificFramework || !other.IsSpecificFramework)
            {
                result = SpecialFrameworkCompare(framework, other);
            }

            if (result == null)
            {
                // PCL compat logic
                if (framework.IsPCL || other.IsPCL)
                {
                    return PCLCompare(framework, other);
                }
                else
                {
                    // regular framework compat check
                    return FrameworkCompare(framework, other);
                }
            }
            else
            {
                return result.Value;
            }
        }

        protected virtual bool? SpecialFrameworkCompare(NuGetFramework framework, NuGetFramework other)
        {
            // TODO: Revist these
            if (framework.IsAny || other.IsAny)
            {
                return true;
            }

            if (framework.IsUnsupported)
            {
                return false;
            }

            if (other.IsAgnostic)
            {
                return true;
            }

            if (other.IsUnsupported)
            {
                return false;
            }

            return null;
        }

        protected virtual bool PCLCompare(NuGetFramework framework, NuGetFramework other)
        {
            // TODO: PCLs can only depend on other PCLs?
            if (framework.IsPCL && !other.IsPCL)
            {
                return false;
            }

            IEnumerable<NuGetFramework> frameworks = null;
            IEnumerable<NuGetFramework> otherFrameworks = null;

            if (framework.IsPCL)
            {
                // do not include optional frameworks here since we might be unable to tell what is optional on the other framework
                _mappings.TryGetPortableFrameworks(framework.Profile, false, out frameworks);
            }
            else
            {
                frameworks = new NuGetFramework[] { framework };
            }

            if (other.IsPCL)
            {
                // include optional frameworks here, the larger the list the more compatible it is
                _mappings.TryGetPortableFrameworks(other.Profile, true, out otherFrameworks);
            }
            else
            {
                otherFrameworks = new NuGetFramework[] { other };
            }

            // check if we this is a compatible superset
            return PCLInnerCompare(frameworks, otherFrameworks);
        }

        private bool PCLInnerCompare(IEnumerable<NuGetFramework> profileFrameworks, IEnumerable<NuGetFramework> otherProfileFrameworks)
        {
            // TODO: Does this check need to make sure multiple frameworks aren't matched against a single framework from the other list?
            return profileFrameworks.Count() <= otherProfileFrameworks.Count() && profileFrameworks.All(f => otherProfileFrameworks.Any(ff => IsCompatible(f, ff)));
        }

        protected virtual bool FrameworkCompare(NuGetFramework framework, NuGetFramework other)
        {
            // find all possible substitutions
            HashSet<NuGetFramework> frameworkSet = new HashSet<NuGetFramework>(NuGetFramework.Comparer) { framework };

            foreach (var fw in _expander.Expand(framework))
            {
                frameworkSet.Add(fw);
            }

            // check all possible substitutions
            foreach (var curFramework in frameworkSet)
            {
                // compare the frameworks
                if (NuGetFramework.FrameworkNameComparer.Equals(curFramework, other)
                    && StringComparer.OrdinalIgnoreCase.Equals(curFramework.Profile, other.Profile)
                    && IsVersionCompatible(curFramework, other))
                {
                    // allow the other if it doesn't have a platform
                    if (other.AnyRuntime)
                    {
                        return true;
                    }
                    else if(curFramework.AnyRuntime)
                    {
                        // Runtime-less frameworks are always incompatible with runtimed frameworks
                        return false;
                    }

                    // compare runtimes
                    return RuntimesCompatible(curFramework.RuntimeIdentifier, other.RuntimeIdentifier);
                }
            }

            return false;
        }

        private bool RuntimesCompatible(string curRuntime, string otherRuntime)
        {
            // Exact match?
            if(string.Equals(curRuntime, otherRuntime, StringComparison.Ordinal))
            {
                return true;
            }

            // Now try to find any match in the graph
            // We can skip the first one since it was checked at the beginning
            return _runtimeGraph.ExpandRuntime(curRuntime).Skip(1).Any(expandedRuntime => string.Equals(expandedRuntime, otherRuntime, StringComparison.Ordinal));
        }

        private bool IsVersionCompatible(NuGetFramework framework, NuGetFramework other)
        {
            return IsVersionCompatible(framework.Version, other.Version);
        }

        private bool IsVersionCompatible(Version framework, Version other)
        {
            return other == FrameworkConstants.EmptyVersion || other <= framework;
        }
    }
}
