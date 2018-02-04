// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Sextant.Domain.Entities;
using System.Collections.Generic;

namespace Sextant.Infrastructure.Repository
{
    public class StarSystemDocument
    {
        public int Id { get; set; }
        public bool Scanned { get; set; }
        public string Name { get; set; }

        public List<CelestialDocument> Celestials { get; set; }

        public StarSystemDocument()
        { }

        public StarSystemDocument(StarSystem system)
        {
            Id         = system.Id;
            Name       = system.Name;
            Scanned    = system.Scanned;
            Celestials = system.Celestials.Select(c => new CelestialDocument(c)).ToList();
        }

        public StarSystem ToEntity()
        {
            return new StarSystem(Id, Name, Celestials.Select(c => c.ToEntity()).ToList());
        }
    }
}
