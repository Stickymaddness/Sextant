// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Events;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class GameLoadCommand : ICommand
    {
        private readonly ICommunicator _communicator;
        private readonly IPlayerStatus _playerStatus;

        private readonly PhraseBook _genericPhrases;

        public string SupportedCommand => "LoadGame";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        public GameLoadCommand(ICommunicator communicator, IPlayerStatus playerStatus, GameLoadPhrases phrases)
        {
            _communicator = communicator;
            _playerStatus = playerStatus;

            _genericPhrases   = PhraseBook.Ingest(phrases.Generic);
        }

        public void Handle(IEvent @event)
        {
            _communicator.Communicate(_genericPhrases.GetRandomPhrase());

            var payload         = @event.Payload;
            double fuelCapacity = (double)payload["FuelCapacity"];

            _playerStatus.SetFuelCapacity(fuelCapacity);
        }
    }
}