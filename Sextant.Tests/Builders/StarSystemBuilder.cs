// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;
using System.Collections.Generic;
using System;

namespace Sextant.Tests.Builders
{
    public class StarSystemBuilder
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public List<Celestial> Celestials { get; private set; }

        public static implicit operator StarSystem(StarSystemBuilder b) => new StarSystem(b.Id, b.Name, b.Celestials);

        public StarSystemBuilder()
        {
            Id         = 1;
            Name       = Guid.NewGuid().ToString();
            Celestials = new List<Celestial>();
        }

        public StarSystemBuilder WithCelestial(Celestial celestial)
        {
            Celestials.Add(celestial);
            return this;
        }

        public StarSystemBuilder WithCelestials(IEnumerable<Celestial> celestials)
        {
            Celestials.AddRange(celestials);
            return this;
        }

        public StarSystemBuilder WithCelestials(params Celestial[] celestials)
        {
            Celestials.AddRange(celestials);
            return this;
        }

        public List<StarSystem> InAList() => new List<StarSystem> { this };
    }
}
