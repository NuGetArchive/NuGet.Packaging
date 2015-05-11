// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Packaging.Core
{
    /// <summary>
    /// Generic packaging exception.
    /// </summary>
    public class PackagingException : Exception
    {
        public PackagingException(string message)
            : base(message)
        {

        }
    }
}
