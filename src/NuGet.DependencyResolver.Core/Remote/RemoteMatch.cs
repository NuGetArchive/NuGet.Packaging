// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using NuGet.LibraryModel;

namespace NuGet.DependencyResolver
{
    public class RemoteMatch
    {
        public IRemoteDependencyProvider Provider { get; set; }
        public LibraryIdentity Library { get; set; }
        public string Path { get; set; }
    }
}