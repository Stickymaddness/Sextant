// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Sextant.Infrastructure.Repository
{
    internal static class NavigationRepositoryExtensions
    {
        internal static List<StarSystemDocument> ToDocument(this IEnumerable<StarSystem> starSystems)
        {
            return starSystems.Select(s => s.ToDocument()).ToList();
        }

        internal static StarSystemDocument ToDocument(this StarSystem starSystem)
        {
            return new StarSystemDocument(starSystem);
        }

        internal static List<StarSystem> ToEntities(this IEnumerable<StarSystemDocument> documents)
        {
            return documents.Select(s => s.ToEntity()).ToList();
        }
    }
}
