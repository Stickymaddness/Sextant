// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Events;
using Sextant.Domain.Entities;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class FindNextSystemCommand : ICommand
    {
        private readonly ICommunicator _communicator;
        private readonly INavigator _navigator;
        private readonly IGalaxyMap _galaxyMap;

        public string SupportedCommand => "find_next_system";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private PhraseBook _phraseBook;

        public FindNextSystemCommand(ICommunicator communicator, INavigator navigator, IGalaxyMap galaxyMap, FindNextSystemPhrases phrases)
        {
            _communicator = communicator;
            _navigator    = navigator;
            _galaxyMap    = galaxyMap;
            _phraseBook   = PhraseBook.Ingest(phrases.Phrases);
        }

        public void Handle(IEvent @event)
        {
            _communicator.Communicate(_phraseBook.GetRandomPhrase());

            StarSystem nextSystem = _navigator.GetNextSystem();

            _galaxyMap.FindSystem(nextSystem.Name);
        }
    }
}
