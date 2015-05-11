// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using NuGet.Packaging.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Packaging
{
    public sealed class PackagesConfigReaderException : PackagingException
    {
        public PackagesConfigReaderException(string message)
            : base(message)
        {

        }
    }
}
