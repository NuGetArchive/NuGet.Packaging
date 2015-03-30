using System;
using System.Globalization;

namespace NuGet.Frameworks
{
    /// <summary>
    /// A simple Target Platform Moniker
    /// Ex: Windows 8.1 
    /// Ex: UAP 10.0
    /// </summary>
    public class NuGetTargetPlatform : IEquatable<NuGetTargetPlatform>
    {
        private readonly string _platform;
        private readonly Version _version;

        private static readonly NuGetTargetPlatform _emptyPlatform 
            = new NuGetTargetPlatform("Empty", FrameworkConstants.EmptyVersion);

        /// <summary>
        /// Target Platform Moniker
        /// </summary>
        /// <param name="platform">Platform identifier</param>
        /// <param name="version">Platform version</param>
        public NuGetTargetPlatform(string platform, Version version)
        {
            if (platform == null)
            {
                throw new ArgumentNullException("platform");
            }

            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            _platform = platform;
            _version = NormalizeVersion(version);
        }

        /// <summary>
        /// Platform identifier
        /// </summary>
        public string Platform
        {
            get
            {
                return _platform;
            }
        }

        /// <summary>
        /// Platform version
        /// </summary>
        public Version Version
        {
            get
            {
                return _version;
            }
        }

        /// <summary>
        /// True if the platform identifiers match and the given platform is a lower than or equal version
        /// </summary>
        public bool IsVersionGreaterOrEqualTo(NuGetTargetPlatform platform)
        {
            return HasSamePlatform(platform) && platform.Version <= Version;
        }

        /// <summary>
        /// True if the platform identifiers match
        /// </summary>
        public bool HasSamePlatform(NuGetTargetPlatform platform)
        {
            return String.Equals(Platform, platform.Platform, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// True if this is the ANY platform.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _emptyPlatform.Equals(this);
            }
        }

        /// <summary>
        /// A special target platform representing Any possible platform.
        /// </summary>
        public static NuGetTargetPlatform Empty
        {
            get
            {
                return _emptyPlatform;
            }
        }

        public bool Equals(NuGetTargetPlatform other)
        {
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            return String.Equals(Platform, other.Platform, StringComparison.OrdinalIgnoreCase)
                && Version.Equals(other.Version);
        }

        public override int GetHashCode()
        {
            HashCodeCombiner combiner = new HashCodeCombiner();

            combiner.AddStringIgnoreCase(Platform);
            combiner.AddObject(Version);

            return combiner.CombinedHash;
        }

        public override bool Equals(object obj)
        {
            NuGetTargetPlatform platform = obj as NuGetTargetPlatform;

            if (obj != null)
            {
                return Equals(platform);
            }

            return false;
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} {1}", Platform, Version);
        }

        private static Version NormalizeVersion(Version version)
        {
            Version normalized = version;

            if (version.Build < 0 || version.Revision < 0)
            {
                normalized = new Version(
                               version.Major,
                               version.Minor,
                               Math.Max(version.Build, 0),
                               Math.Max(version.Revision, 0));
            }

            return normalized;
        }
    }
}