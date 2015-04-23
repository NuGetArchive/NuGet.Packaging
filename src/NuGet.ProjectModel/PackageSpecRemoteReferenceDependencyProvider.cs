// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.DependencyResolver;
using NuGet.Frameworks;
using NuGet.LibraryModel;

namespace NuGet.ProjectModel
{
    public class PackageSpecRemoteReferenceDependencyProvider : IRemoteDependencyProvider
    {
        private readonly IPackageSpecResolver _resolver;

        public PackageSpecRemoteReferenceDependencyProvider(IPackageSpecResolver projectResolver)
        {
            _resolver = projectResolver;
        }

        public bool IsHttp => false;

        public Task CopyToAsync(LibraryIdentity match, Stream stream, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<LibraryIdentity> FindLibraryAsync(LibraryRange libraryRange, NuGetFramework targetFramework, CancellationToken cancellationToken)
        {
            PackageSpec packageSpec;

            // Can't find a project file with the name so bail
            if (!_resolver.TryResolvePackageSpec(libraryRange.Name, out packageSpec))
            {
                return Task.FromResult<LibraryIdentity>(null);
            }

            return Task.FromResult(new LibraryIdentity
            {
                Name = libraryRange.Name,
                Version = packageSpec.Version,
                Type = LibraryTypes.Project
            });
        }

        public Task<IEnumerable<LibraryDependency>> GetDependenciesAsync(LibraryIdentity match, NuGetFramework targetFramework, CancellationToken cancellationToken)
        {
            PackageSpec packageSpec;

            // Can't find a project file with the name so bail
            if (!_resolver.TryResolvePackageSpec(match.Name, out packageSpec))
            {
                return Task.FromResult<IEnumerable<LibraryDependency>>(null);
            }

            // This never returns null
            var targetFrameworkInfo = packageSpec.GetTargetFramework(targetFramework);
            var targetFrameworkDependencies = targetFrameworkInfo.Dependencies;

            if (targetFramework.IsDesktop())
            {
                targetFrameworkDependencies.Add(new LibraryDependency
                {
                    LibraryRange = new LibraryRange
                    {
                        Name = "mscorlib",
                        TypeConstraint = LibraryTypes.Reference
                    }
                });

                targetFrameworkDependencies.Add(new LibraryDependency
                {
                    LibraryRange = new LibraryRange
                    {
                        Name = "System",
                        TypeConstraint = LibraryTypes.Reference
                    }
                });

                targetFrameworkDependencies.Add(new LibraryDependency
                {
                    LibraryRange = new LibraryRange
                    {
                        Name = "System.Core",
                        TypeConstraint = LibraryTypes.Reference
                    }
                });

                targetFrameworkDependencies.Add(new LibraryDependency
                {
                    LibraryRange = new LibraryRange
                    {
                        Name = "Microsoft.CSharp",
                        TypeConstraint = LibraryTypes.Reference
                    }
                });
            }

            return Task.FromResult(packageSpec.Dependencies.Concat(targetFrameworkDependencies));
        }
    }
}
