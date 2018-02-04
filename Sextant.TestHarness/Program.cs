// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Host;
using System;

namespace Sextant.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            SextantHost sextant = new SextantHost(basePath: Environment.CurrentDirectory, pluginName: "TestHarness");
            sextant.Initialize();

            Console.ReadKey();
        }
    }
}
