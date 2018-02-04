// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;
using Sextant.Domain.Events;
using Sextant.Domain.Phrases;
using System.Linq;

namespace Sextant.Domain.Commands
{
    public class NextScanCommand : ICommand
    {
        private readonly INavigator    _navigator;
        private readonly ICommunicator _communicator;
        private readonly IPlayerStatus _playerStatus;

        public string SupportedCommand     => "next_scan";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private PhraseBook _skipPhrases;
        private PhraseBook _nextScanPhrases;
        private PhraseBook _completePhrases;

        public NextScanCommand(ICommunicator communicator, INavigator navigator, IPlayerStatus playerStatus, NextScanPhrases phrases)
        {
            _navigator       = navigator;
            _communicator    = communicator;
            _playerStatus    = playerStatus;

            _skipPhrases     = PhraseBook.Ingest(phrases.SkipSystem);
            _nextScanPhrases = PhraseBook.Ingest(phrases.NextScan);
            _completePhrases = PhraseBook.Ingest(phrases.ScansComplete);
        }

        public void Handle(IEvent @event)
        {
            string currentSystem = _playerStatus.Location;
            StarSystem system    = _navigator.GetSystem(currentSystem);

            if (system == null)
            {
                _communicator.Communicate(_skipPhrases.GetRandomPhrase());
                return;
            }

            Celestial nextToScan = system.Celestials
                                         .Where(c => c.Scanned == false)
                                         .OrderBy(r => r.ShortName)
                                         .FirstOrDefault();

            if (nextToScan == null)
            {
                _communicator.Communicate(_completePhrases.GetRandomPhrase());
                return;
            }

            _communicator.Communicate(_nextScanPhrases.GetRandomPhraseWith(nextToScan.ShortName));
        }
    }
}
