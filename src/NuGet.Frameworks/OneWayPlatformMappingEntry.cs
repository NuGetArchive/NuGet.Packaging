using System;
using System.Globalization;

namespace NuGet.Frameworks
{
    public class OneWayPlatformMappingEntry : IEquatable<OneWayPlatformMappingEntry>
    {
        private readonly NuGetTargetPlatform _platform;
        private readonly NuGetTargetPlatform _compatiblePlatform;

        /// <summary>
        /// Represents a one way compatibility mapping between target platforms.
        /// </summary>
        public OneWayPlatformMappingEntry(NuGetTargetPlatform platform, NuGetTargetPlatform compatiblePlatform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException("platform");
            }

            if (compatiblePlatform == null)
            {
                throw new ArgumentNullException("compatiblePlatform");
            }

            _platform = platform;
            _compatiblePlatform = compatiblePlatform;
        }

        /// <summary>
        /// Target platform supporting the CompatiblePlatform
        /// </summary>
        public NuGetTargetPlatform Platform
        {
            get
            {
                return _platform;
            }
        }

        /// <summary>
        /// Target platform compatible with Platform
        /// </summary>
        public NuGetTargetPlatform CompatiblePlatform
        {
            get
            {
                return _compatiblePlatform;
            }
        }

        public bool Equals(OneWayPlatformMappingEntry other)
        {
            if (other == null)
            {
                return false;
            }

            return Platform.Equals(other.Platform) && CompatiblePlatform.Equals(other.CompatiblePlatform);
        }

        public override int GetHashCode()
        {
            HashCodeCombiner combiner = new HashCodeCombiner();

            combiner.AddObject(Platform);
            combiner.AddObject(CompatiblePlatform);

            return combiner.CombinedHash;
        }

        public override bool Equals(object obj)
        {
            OneWayPlatformMappingEntry other = obj as OneWayPlatformMappingEntry;

            if (other != null)
            {
                return Equals(other);
            }

            return false;
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} -> {1}", Platform, CompatiblePlatform);
        }
    }
}