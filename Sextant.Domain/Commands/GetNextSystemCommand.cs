// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;
using Sextant.Domain.Events;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class GetNextSystemCommand : ICommand
    {
        public string SupportedCommand     => "get_next_system";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private readonly ICommunicator _communicator;
        private readonly INavigator _navigator;

        private PhraseBook _phraseBook;

        public GetNextSystemCommand(ICommunicator communicator, INavigator navigator, GetNextSystemPhrases phrases)
        {
            _communicator = communicator;
            _navigator    = navigator;
            _phraseBook   = PhraseBook.Ingest(phrases.Phrases);
        }

        public void Handle(IEvent @event)
        {
            StarSystem nextSystem = _navigator.GetNextSystem();

            _communicator.Communicate(string.Format(_phraseBook.GetRandomPhrase(), nextSystem.Name));
        }
    }
}
