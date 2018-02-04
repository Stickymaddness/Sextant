// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Collections.Generic;
using Sextant.Domain.Entities;

namespace Sextant.Domain
{
    public class Navigator : INavigator
    {
        private readonly INavigationRepository _navigationRepository;

        public Navigator(INavigationRepository navigationRepository)
        {
            _navigationRepository = navigationRepository;
        }

        public bool ExpeditionComplete => _navigationRepository.GetSystems().All(s => s.Scanned);
        public bool ExpeditionStarted  => !_navigationRepository.IsEmpty();

        public bool CancelExpedition() => _navigationRepository.Clear();

        public bool PlanExpedition(IEnumerable<StarSystem> systems)
        {
            if (ExpeditionStarted)
                return false;

            return ExtendExpedition(systems);
        }

        public bool ExtendExpedition(IEnumerable<StarSystem> systems)
        {
            if (systems == null || !systems.Any())
                return false;

            return _navigationRepository.Store(systems);
        }

        public StarSystem GetNextSystem()
        {
            return _navigationRepository.GetFirstUnscannedSystem();
        }

        public Celestial GetNextCelestial()
        {
            return GetNextSystem()?.Celestials.FirstOrDefault(c => c.Scanned == false);
        }

        public bool ScanCelestial(string celestial)
        {
            return _navigationRepository.ScanCelestial(celestial);
        }

        public bool ScanSystem(string system)
        {
            return _navigationRepository.ScanSystem(system);
        }

        public bool UnscanSystem(string system)
        {
            return _navigationRepository.UnscanSystem(system);
        }

        public bool SystemScanned(string system)
        {
            return _navigationRepository.GetSystem(system).Scanned;
        }

        public int SystemsRemaining()
        {
            return _navigationRepository.GetUnscannedSystems().Count();
        }

        public int CelestialsRemaining()
        {
            return GetAllRemainingCelestials().Count();
        }

        public List<Celestial> GetAllRemainingCelestials()
        {
            return _navigationRepository.GetUnscannedSystems().SelectMany(s => s.Celestials.Where(c => c.Scanned == false)).ToList();
        }

        public StarSystem GetSystem(string systemName)
        {
            return _navigationRepository.GetSystem(systemName);
        }

        public List<StarSystem> GetAllExpeditionSystems()
        {
            return _navigationRepository.GetAllExpeditionStarSystems();
        }

        public List<Celestial> GetRemainingCelestials(string systemName)
        {
            return _navigationRepository.GetSystem(systemName).Celestials.Where(c => c.Scanned == false).ToList();
        }

        public bool SystemInExpedition(string system)
        {
            return _navigationRepository.GetSystem(system) != null;
        }
    }
}