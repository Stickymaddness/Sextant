// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Sextant.Domain.Events;
using System.Collections.Generic;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class CelestialScanCommand : ICommand
    {
        private const string ScanCompletePhrases = "ScanComplete";
        private readonly INavigator    _navigator;
        private readonly ICommunicator _communicator;
        private readonly IPlayerStatus _playerStatus;

        public string SupportedCommand     => "Scan";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private readonly PhraseBook _scanCompletePhrases;
        private readonly PhraseBook _oneRemainingPhrases;
        private readonly PhraseBook _allScansCompletePhrases;
        private readonly PhraseBook _multipleRemainingPhrases;
        private readonly PhraseBook _expeditionCompletePhrases;

        public CelestialScanCommand(ICommunicator communicator, INavigator navigator, IPlayerStatus playerStatus, CelestialScanPhrases phrases)
        {
            _communicator              = communicator;
            _navigator                 = navigator;
            _playerStatus              = playerStatus;

            _scanCompletePhrases       = PhraseBook.Ingest(phrases.ScanComplete);
            _allScansCompletePhrases   = PhraseBook.Ingest(phrases.AllScansComplete);
            _oneRemainingPhrases       = PhraseBook.Ingest(phrases.SingleScanRemaining);
            _multipleRemainingPhrases  = PhraseBook.Ingest(phrases.MultipleScansRemaining);
            _expeditionCompletePhrases = PhraseBook.Ingest(phrases.ExpeditionComplete);
        }

        public void Handle(IEvent @event)
        {
            Dictionary<string, object> eventPayload = @event.Payload;
            string currentSystem                    = _playerStatus.Location;
            bool expeditionSystem                   = _navigator.SystemInExpedition(currentSystem);

            _navigator.ScanCelestial(eventPayload["BodyName"].ToString());

            string script = BuildScript(currentSystem, expeditionSystem);

            _communicator.Communicate(script);
        }

        private string BuildScript(string currentSystem, bool expeditionSystem)
        {
            string script = _scanCompletePhrases.GetRandomPhrase();

            if (!expeditionSystem)
                return script;

            if (_navigator.GetSystem(currentSystem).Celestials.Any(c => c.Scanned == false))
            {
                int scansRemaining = _navigator.GetRemainingCelestials(currentSystem).Count();

                if (scansRemaining == 1)
                    return script += _oneRemainingPhrases.GetRandomPhrase();

                return script += _multipleRemainingPhrases.GetRandomPhraseWith(scansRemaining);
            }

            if (_navigator.ExpeditionComplete)
                return script += _expeditionCompletePhrases.GetRandomPhrase();

            return script += _allScansCompletePhrases.GetRandomPhrase();
        }
    }
}