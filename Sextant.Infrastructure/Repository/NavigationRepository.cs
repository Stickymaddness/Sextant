// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Sextant.Domain;
using System.Collections.Generic;
using Sextant.Domain.Entities;
using System;

namespace Sextant.Infrastructure.Repository
{
    public class NavigationRepository : INavigationRepository, IDisposable
    {
        private readonly IDataStore<StarSystemDocument> _repository;

        public NavigationRepository(IDataStore<StarSystemDocument> repository)
        {
            _repository = repository;
        }

        public bool Clear()                                   => _repository.Drop();
        public bool IsEmpty()                                 => _repository.Count() == 0;
        public void Dispose()                                 => _repository.Dispose();
        public bool ScanSystem(string system)                 => UpdateSystem(system, true);
        public bool UnscanSystem(string system)               => UpdateSystem(system, false);
        public List<StarSystem> GetSystems()                  => _repository.FindAll().ToEntities().ToList();
        public StarSystem GetLastScannedSystem()              => _repository.Find(x => x.Scanned).Last().ToEntity();
        public StarSystem GetSystem(string name)              => _repository.FindOne(s => s.Name.ToUpper() == name.ToUpper())?.ToEntity();
        public StarSystem GetFirstUnscannedSystem()           => _repository.FindOne(x => x.Scanned == false)?.ToEntity();
        public List<StarSystem> GetUnscannedSystems()         => _repository.Find(x => x.Scanned == false).ToEntities().ToList();
        public List<StarSystem> GetAllExpeditionStarSystems() => _repository.FindAll().ToEntities().ToList();

        private bool UpdateSystem(string system, bool scanned)
        {
            StarSystemDocument document = _repository.FindOne(s => s.Name.ToUpper() == system.ToUpper());

            if (document == null)
                return false;

            document.Celestials.ForEach(c => c.Scanned = scanned);
            document.Scanned = scanned;

            return _repository.Update(document);
        }

        public bool ScanCelestial(string celestial)
        {
            StarSystemDocument document = _repository.FindOne(s => s.Celestials.FirstOrDefault(c => c.Name == celestial) != null);

            if (document == null)
                return false;

            document.Celestials.Single(c => c.Name.ToUpper() == celestial.ToUpper()).Scanned = true;

            if (document.Celestials.All(c => c.Scanned))
                document.Scanned = true;

            return _repository.Update(document);
        }

        public bool Store(IEnumerable<StarSystem> starSystems)
        {
            _repository.InsertBulk(starSystems.ToDocument());
            return true;
        }
    }
}