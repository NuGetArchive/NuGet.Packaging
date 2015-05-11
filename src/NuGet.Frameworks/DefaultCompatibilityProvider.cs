// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Frameworks
{
    public sealed class DefaultCompatibilityProvider : CompatibilityProvider
    {

        public DefaultCompatibilityProvider()
            : base(DefaultFrameworkNameProvider.Instance)
        {

        }

        private static IFrameworkCompatibilityProvider _instance;
        public static IFrameworkCompatibilityProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DefaultCompatibilityProvider();
                }

                return _instance;
            }
        }

    }
}
