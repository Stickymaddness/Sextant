// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;
using System.Collections.Generic;

namespace Sextant.Domain
{
    public interface INavigationRepository
    {
        bool Clear();
        bool IsEmpty();
        bool ScanCelestial(string celestial);
        bool ScanSystem(string system);
        bool UnscanSystem(string system);
        bool Store(IEnumerable<StarSystem> starSystem);

        StarSystem GetSystem(string name);
        StarSystem GetLastScannedSystem();
        List<StarSystem> GetSystems();

        StarSystem GetFirstUnscannedSystem();
        List<StarSystem> GetUnscannedSystems();
        List<StarSystem> GetAllExpeditionStarSystems();
    }
}
