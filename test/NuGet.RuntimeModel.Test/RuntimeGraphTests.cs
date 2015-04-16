using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NuGet.RuntimeModel.Test
{
    public class RuntimeGraphTests
    {
        [Fact]
        public void MergingInEmptyGraphHasNoEffect()
        {
            var graph = new RuntimeGraph(new[] {
                new RuntimeDescription("any"),
                new RuntimeDescription("win8", new[] { "any" })
                });
            graph.MergeIn(new RuntimeGraph());
            Assert.Equal(new RuntimeGraph(new[] {
                new RuntimeDescription("any"),
                new RuntimeDescription("win8", new[] { "any" })
                }), graph);
        }

        [Fact]
        public void MergingAddsCompletelyNewRuntimes()
        {
            var leftGraph = new RuntimeGraph(new[] {
                new RuntimeDescription("any"),
                new RuntimeDescription("win8", new[] { "any", "win7" })
                });
            var rightGraph = new RuntimeGraph(new[]
            {
                new RuntimeDescription("win7")
            });
            leftGraph.MergeIn(rightGraph);
            Assert.Equal(new RuntimeGraph(new[] {
                new RuntimeDescription("any"),
                new RuntimeDescription("win8", new[] { "any", "win7" }),
                new RuntimeDescription("win7")
                }), leftGraph);
        }

        [Fact]
        public void MergingCombinesDependencySetsInRuntimesDefinedInBoth()
        {
            var leftGraph = new RuntimeGraph(new[] {
                new RuntimeDescription("any"),
                new RuntimeDescription("win8", new[] { "any", "win7" }, new [] {
                    new RuntimeDependencySet("Foo"),
                })
            });
            var rightGraph = new RuntimeGraph(new[]
            {
                new RuntimeDescription("win8", new[] {
                    new RuntimeDependencySet("Bar")
                })
            });
            leftGraph.MergeIn(rightGraph);
            Assert.Equal(new RuntimeGraph(new[] {
                new RuntimeDescription("any"),
                new RuntimeDescription("win8", new[] { "any", "win7" }, new [] {
                    new RuntimeDependencySet("Foo"),
                    new RuntimeDependencySet("Bar")
                }),
            }), leftGraph);
        }

        [Fact]
        public void MergingCombinesDependenciesInDependencySetsDefinedInBoth()
        {
            var leftGraph = new RuntimeGraph(new[] {
                new RuntimeDescription("any"),
                new RuntimeDescription("win8", new[] { "any", "win7" }, new [] {
                    new RuntimeDependencySet("Foo", new [] {
                        new RuntimePackageDependency("Foo.win8", new NuGetVersion(1, 2, 3))
                    }),
                })
            });
            var rightGraph = new RuntimeGraph(new[]
            {
                new RuntimeDescription("win8", new[] {
                    new RuntimeDependencySet("Foo", new [] {
                        new RuntimePackageDependency("Foo.more.win8", new NuGetVersion(4, 5, 6))
                    }),
                })
            });
            leftGraph.MergeIn(rightGraph);
            Assert.Equal(new RuntimeGraph(new[] {
                new RuntimeDescription("any"),
                new RuntimeDescription("win8", new[] { "any", "win7" }, new [] {
                    new RuntimeDependencySet("Foo", new [] {
                        new RuntimePackageDependency("Foo.win8", new NuGetVersion(1, 2, 3)),
                        new RuntimePackageDependency("Foo.more.win8", new NuGetVersion(4, 5, 6))
                    }),
                }),
            }), leftGraph);
        }
    }
}
