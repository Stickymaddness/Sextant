// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;

namespace Sextant.Infrastructure.Repository
{
    public class CelestialDocument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string System { get; set; }
        public string Clasification { get; set; }
        public bool Scanned { get; set; }
        public bool Landable { get; set; }

        public CelestialDocument()
        { }

        public CelestialDocument(Celestial celetial)
        {
            Id            = celetial.Id;
            Name          = celetial.Name;
            System        = celetial.System;
            Clasification = celetial.Clasification;
            Scanned       = celetial.Scanned;
            Landable      = celetial.Landable;
        }

        public Celestial ToEntity()
        {
            return new Celestial(Name, Clasification, System, Scanned, Id);
        }
    }
}
