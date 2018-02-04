// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Sextant.Domain.Entities
{
    public class Celestial
    {
        public int Id { get; set; }

        public bool Scanned { get; }

        public string Name { get; }
        public string System { get; }
        public string Clasification { get; }
        public bool Landable { get; }

        public string ShortName => Name.Replace(System, string.Empty);

        public Celestial(string name, string clasification, string system, bool scanned = false, int id = 0)
        {
            Id            = id;
            Name          = name;
            System        = system;
            Clasification = clasification;
            Scanned       = scanned;
        }
    }
}
