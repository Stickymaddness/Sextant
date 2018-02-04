// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;
using System.Collections.Generic;

namespace Sextant.Domain
{
    public interface INavigator
    {
        bool ExpeditionComplete { get; }
        bool ExpeditionStarted { get; }

        Celestial GetNextCelestial();
        StarSystem GetNextSystem();
        StarSystem GetSystem(string systemName);

        int SystemsRemaining();
        int CelestialsRemaining();

        bool PlanExpedition(IEnumerable<StarSystem> systems);
        bool ExtendExpedition(IEnumerable<StarSystem> systems);

        List<Celestial> GetRemainingCelestials(string systemName);
        List<Celestial> GetAllRemainingCelestials();
        List<StarSystem> GetAllExpeditionSystems();

        bool CancelExpedition();
        bool ScanCelestial(string celestial);
        bool ScanSystem(string system);
        bool UnscanSystem(string system);
        bool SystemInExpedition(string currentSystem);
    }
}