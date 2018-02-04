// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System;
using Sextant.Domain.Events;
using System.Collections.Generic;
using Sextant.Domain.Entities;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class ProgressCommand : ICommand
    {
        public string SupportedCommand     => "expedition_progress";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private readonly ICommunicator _communicator;
        private readonly INavigator _navigator;
        private readonly IPlayerStatus _playerStatus;

        private PhraseBook _progressPhrases;
        private PhraseBook _systemsScannedPhrases;

        public ProgressCommand(ICommunicator communicator, INavigator navigator, IPlayerStatus playerStatus, ProgressPhrases phrases)
        {
            _navigator             = navigator;
            _communicator          = communicator;
            _playerStatus          = playerStatus;

            _progressPhrases       = PhraseBook.Ingest(phrases.Progress);
            _systemsScannedPhrases = PhraseBook.Ingest(phrases.SystemsScanned);
        }

        public void Handle(IEvent @event)
        {
            List<StarSystem> systems  = _navigator.GetAllExpeditionSystems();

            double totalSystems       = systems.Count;
            double scannedSystems     = systems.Where(s => s.Scanned).Count();

            double progressPercentage = Math.Round(scannedSystems / totalSystems * 100);

            TimeSpan expeditionLength = _playerStatus.ExpeditionLength;

            _communicator.Communicate(_progressPhrases.GetRandomPhraseWith(expeditionLength.Days, expeditionLength.Hours, expeditionLength.Minutes, progressPercentage));
            _communicator.Communicate(_systemsScannedPhrases.GetRandomPhraseWith(scannedSystems, totalSystems));
        }
    }
}
