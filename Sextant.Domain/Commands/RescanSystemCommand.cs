// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Events;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class RescanSystemCommand : ICommand
    {
        private readonly INavigator _navigator;
        private readonly ICommunicator _communicator;
        private readonly IPlayerStatus _playerStatus;
        private readonly PhraseBook _systemUnscanned;
        private readonly PhraseBook _notExpeditionSystem;
        private readonly string _errorPhrase;

        public string SupportedCommand => "rescan_system";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        public RescanSystemCommand(ICommunicator communicator, INavigator navigator, IPlayerStatus playerStatus, RescanSystemPhrases phrases, ScansRemainingPhrases scanPhrases)
        {
            _navigator           = navigator;
            _communicator        = communicator;
            _playerStatus        = playerStatus;

            _errorPhrase         = phrases.Error;
            _systemUnscanned     = PhraseBook.Ingest(phrases.SystemUnscanned);
            _notExpeditionSystem = PhraseBook.Ingest(scanPhrases.SkipSystem);
        }

        public void Handle(IEvent @event)
        {
            string currentSystem = _playerStatus.Location;

            if (!_navigator.SystemInExpedition(currentSystem))
            {
                _communicator.Communicate(_notExpeditionSystem.GetRandomPhrase());
                return;
            }

            bool success  = _navigator.UnscanSystem(currentSystem);
            string script = success ? _systemUnscanned.GetRandomPhrase() : _errorPhrase;

            _communicator.Communicate(script);
        }
    }
}
