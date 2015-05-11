// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.RuntimeModel;

namespace NuGet.DependencyResolver
{
    public interface IRemoteDependencyProvider
    {
        bool IsHttp { get; }

        Task<RemoteMatch> FindLibrary(LibraryRange libraryRange, NuGetFramework targetFramework);
        Task<IEnumerable<LibraryDependency>> GetDependencies(RemoteMatch match, NuGetFramework targetFramework);
        Task CopyToAsync(RemoteMatch match, Stream stream);
        Task<RuntimeGraph> GetRuntimeGraph(RemoteMatch match, NuGetFramework framework);
    }

}