// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Sextant.Domain.Entities
{
    public class StarSystem
    {
        public int Id { get; private set; }

        public List<Celestial> Celestials { get; private set; }

        public bool Scanned => Celestials.All(c => c.Scanned);
        public string Name { get; private set; }

        public StarSystem(string name)
        {
            Name       = name;
            Celestials = new List<Celestial>();
        }

        public StarSystem(int id, string name, List<Celestial> celestials)
        {
            Id         = id;
            Name       = name;
            Celestials = celestials;
        }

        public void AddCelestial(string name, string clasification)
        {
            Celestials.Add(new Celestial(name, clasification, Name));
        }
    }
}
