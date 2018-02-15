// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using System;

namespace Sextant.Infrastructure.Repository
{
    public class PlayerStatusRepository : IPlayerStatus, IDisposable
    {
        private const int playerStatusId = 1;

        private static readonly object sync = new object();

        private PlayerStatus _playerStatus;
        private readonly IDataStore<PlayerStatus> _dataStore;

        public string Location           => _playerStatus.Location;
        public double FuelCapacity       => _playerStatus.FuelCapacity;
        public TimeSpan ExpeditionLength => DateTimeOffset.Now - _playerStatus.ExpeditionStart;

        public void Dispose() => _dataStore.Dispose();

        public PlayerStatusRepository(IDataStore<PlayerStatus> dataStore)
        {
            _dataStore                      = dataStore;
            PlayerStatus storedPlayerStatus = _dataStore.FindOne(p => p.Id == playerStatusId);
            _playerStatus                   = storedPlayerStatus;

            if (_playerStatus == null)
            {
                _playerStatus = new PlayerStatus(playerStatusId);
                _dataStore.Insert(_playerStatus);
            }
        }

        private bool Store()
        {
            lock(sync)
                return _dataStore.Update(_playerStatus);
        }

        public bool SetExpeditionStart(DateTimeOffset start)
        {
            _playerStatus.ExpeditionStart = start;
            return Store();
        }

        public bool SetFuelCapacity(double capacity)
        {
            if (_playerStatus.FuelCapacity == capacity)
                return true;

            _playerStatus.FuelCapacity = capacity;
            return Store();
        }

        public bool SetLocation(string location)
        {
            if (_playerStatus.Location == location)
                return true;

            _playerStatus.Location = location;
            return Store();
        }
    }
}
