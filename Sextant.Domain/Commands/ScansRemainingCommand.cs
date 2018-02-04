// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Sextant.Domain.Events;
using Sextant.Domain.Entities;
using System.Collections.Generic;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class ScansRemainingCommand : ICommand
    {
        public string SupportedCommand     => "scans_remaining";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private readonly INavigator    _navigator;
        private readonly ICommunicator _communicator;
        private readonly IPlayerStatus _playerStatus;

        private readonly string     _andPhrase;
        private readonly string     _planetPhrase;
        private readonly PhraseBook _skipPhraseBook;
        private readonly PhraseBook _completePhrases;
        private readonly PhraseBook _remainingPhrases;


        public ScansRemainingCommand(ICommunicator communicator, INavigator navigator, IPlayerStatus playerStatus, ScansRemainingPhrases phrases)
        {
            _communicator     = communicator;
            _navigator        = navigator;
            _playerStatus     = playerStatus;
            _andPhrase        = phrases.AndPhrase;
            _planetPhrase     = phrases.PlanetPhrase;

            _skipPhraseBook   = PhraseBook.Ingest(phrases.SkipSystem);
            _completePhrases  = PhraseBook.Ingest(phrases.SystemComplete);
            _remainingPhrases = PhraseBook.Ingest(phrases.ScansRemaining);
        }

        public void Handle(IEvent @event)
        {
            string currentSystem = _playerStatus.Location;

            IEnumerable<Celestial> remaining = _navigator.GetSystem(currentSystem)?
                                                         .Celestials
                                                         .Where(c => c.Scanned == false)
                                                         .OrderBy(r => r.ShortName);

            if (remaining == null)
            {
                _communicator.Communicate(_skipPhraseBook.GetRandomPhrase());
                return;
            }

            if (!remaining.Any())
            {
                _communicator.Communicate(_completePhrases.GetRandomPhrase());
                return;
            }

            string script = string.Empty;

            foreach (Celestial celestial in remaining)
            {
                if (celestial == remaining.Last() && remaining.Count() > 1)
                    script += $" {_andPhrase} ";

                script += $"{_planetPhrase} {celestial.ShortName}. ";
            }

            string finalScript = string.Format(_remainingPhrases.GetRandomPhrase(), remaining.Count(), script);

            _communicator.Communicate(finalScript);
        } 
    }
}
