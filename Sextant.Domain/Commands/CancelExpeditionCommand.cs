// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Events;
using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class CancelExpeditionCommand : ICommand
    {
        public string SupportedCommand     => "cancel_expedition";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private readonly ICommunicator _communicator;
        private readonly INavigator _navigator;

        private readonly string _warning;
        private readonly string _complete;
        private readonly string _error;

        private static int _executionCount = 0;

        public CancelExpeditionCommand(ICommunicator communicator, INavigator navigator, CancelExpeditionPhrases phrases)
        {
            _communicator = communicator;
            _navigator    = navigator;

            _warning      = phrases.Warning;
            _error        = phrases.Error;
            _complete     = phrases.Complete;
        }

        public void Handle(IEvent @event)
        {
            if (_executionCount == 0)
            {
                _executionCount++;
                _communicator.Communicate(_warning);
            }
            else if (_executionCount == 1)
            {
                _executionCount = 0;

                bool success    = _navigator.CancelExpedition();
                string script   = success ? _complete : _error;

                _communicator.Communicate(script);
            }
        }
    }
}
