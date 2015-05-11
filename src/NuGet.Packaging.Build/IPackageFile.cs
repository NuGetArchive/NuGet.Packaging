﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.IO;

namespace NuGet.Packaging.Build
{
    public interface IPackageFile
    {
        int? Version { get; }
        string Path { get; }
        Stream GetStream();
    }
}