// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using System.Collections.Generic;

namespace Sextant.Tests
{
    internal class TestGalaxyMap : IGalaxyMap
    {
        public List<string> Systems { get; } = new List<string>();

        public void FindSystem(string system)
        {
            Systems.Add(system);
        }
    }
}
