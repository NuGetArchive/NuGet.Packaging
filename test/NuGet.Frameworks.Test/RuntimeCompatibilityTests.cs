using NuGet.RuntimeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NuGet.Frameworks.Test
{
    public class RuntimeCompatibilityTests
    {
        [Fact]
        public void WithoutRuntimeGraph_DifferentRuntimesAreConsideredIncompatible()
        {
            var framework1 = NuGetFramework.Parse("core50~win8");
            var framework2 = NuGetFramework.Parse("core50~win8-x86");

            var compat = DefaultCompatibilityProvider.Instance;

            Assert.False(compat.IsCompatible(framework1, framework2));
        }

        [Fact]
        public void WithoutRuntimeGraph_SameRuntimeIsCompatible()
        {
            var framework1 = NuGetFramework.Parse("core50~win8");
            var framework2 = NuGetFramework.Parse("core50~win8");

            var compat = DefaultCompatibilityProvider.Instance;

            Assert.True(compat.IsCompatible(framework1, framework2));
        }

        [Fact]
        public void FrameworkWithRuntime_IsAlwaysCompatibleWithFrameworkWithoutRuntime()
        {
            var framework1 = NuGetFramework.Parse("core50~win8"); // The framework of the project
            var framework2 = NuGetFramework.Parse("core50");      // The framework of the package

            var compat = DefaultCompatibilityProvider.Instance;

            Assert.True(compat.IsCompatible(framework1, framework2));
        }

        [Fact]
        public void FrameworkWithoutRuntime_IsNotCompatibleWithFrameworkHavingRuntime()
        {
            var framework1 = NuGetFramework.Parse("core50");        // The framework of the project
            var framework2 = NuGetFramework.Parse("core50~win8");   // The framework of the package

            var compat = DefaultCompatibilityProvider.Instance;

            Assert.False(compat.IsCompatible(framework1, framework2));
        }

        [Theory]
        [InlineData("core50~win8-x86", "core50~win7")]
        [InlineData("core50~win8-x86", "core50~win8")]
        [InlineData("core50~win8-x86", "core50~win7-x86")]
        public void WithRuntimeGraph_ParentRuntimesAreCompatibleWithChildRuntimes(string fw1, string fw2)
        {
            var graph = new RuntimeGraph(new[]
            {
                new RuntimeDescription("win8-x86", new [] { "win8", "win7-x86" }),
                new RuntimeDescription("win8", new [] { "win7" }),
                new RuntimeDescription("win7-x86", new [] { "win7" }),
                new RuntimeDescription("win7"),
            });

            var framework1 = NuGetFramework.Parse(fw1); // The framework of the project
            var framework2 = NuGetFramework.Parse(fw2); // The framework of the package

            var compat = DefaultCompatibilityProvider.WithRuntimeGraph(graph);

            Assert.True(compat.IsCompatible(framework1, framework2));
        }

        [Theory]
        [InlineData("core50~win7", "core50~win8-x86")]
        [InlineData("core50~win8", "core50~win8-x86")]
        [InlineData("core50~win7-x86", "core50~win8-x86")]
        public void WithRuntimeGraph_ChildRuntimesAreIncompatibleWithParentRuntimes(string fw1, string fw2)
        {
            var graph = new RuntimeGraph(new[]
            {
                new RuntimeDescription("win8-x86", new [] { "win8", "win7-x86" }),
                new RuntimeDescription("win8", new [] { "win7" }),
                new RuntimeDescription("win7-x86", new [] { "win7" }),
                new RuntimeDescription("win7"),
            });

            var framework1 = NuGetFramework.Parse(fw1); // The framework of the project
            var framework2 = NuGetFramework.Parse(fw2); // The framework of the package

            var compat = DefaultCompatibilityProvider.WithRuntimeGraph(graph);

            Assert.False(compat.IsCompatible(framework1, framework2));
        }

        [Fact]
        public void DifferentButCompatibleFrameworksAreCompatibleIfRuntimesAreTheSame()
        {
            var framework1 = NuGetFramework.Parse("dnxcore50~win8");
            var framework2 = NuGetFramework.Parse("core50~win8");

            var compat = DefaultCompatibilityProvider.Instance;

            Assert.True(compat.IsCompatible(framework1, framework2));
        }

        [Fact]
        public void DifferentButCompatibleFrameworksAreCompatibleIfRuntimesAreCompatible()
        {
            var graph = new RuntimeGraph(new[]
            {
                new RuntimeDescription("win8-x86", new [] { "win8", "win7-x86" }),
                new RuntimeDescription("win8", new [] { "win7" }),
                new RuntimeDescription("win7-x86", new [] { "win7" }),
                new RuntimeDescription("win7"),
            });

            var framework1 = NuGetFramework.Parse("dnxcore50~win8-x86");
            var framework2 = NuGetFramework.Parse("core50~win8");

            var compat = DefaultCompatibilityProvider.WithRuntimeGraph(graph);

            Assert.True(compat.IsCompatible(framework1, framework2));
        }
    }
}
