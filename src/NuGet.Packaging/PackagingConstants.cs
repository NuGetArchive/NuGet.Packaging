// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NuGet.Packaging
{
    public static class PackagingConstants
    {
        public const string ContentFolder = "content";
        public const string AnyFramework = "any";
        public const string AgnosticFramework = "agnostic";

        public const string TargetFrameworkPropertyKey = "targetframework";
    }
}
