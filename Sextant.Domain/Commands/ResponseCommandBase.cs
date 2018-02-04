// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Events;

namespace Sextant.Domain.Commands
{
    public abstract class ResponseCommandBase : ICommand
    {
        private readonly ICommunicator _communicator;
        internal abstract string _supportedCommand { get; }
        internal abstract PhraseBook _phraseBook { get; }

        public bool Handles(IEvent @event) => @event.Event == _supportedCommand;
        public string SupportedCommand => _supportedCommand;

        public ResponseCommandBase(ICommunicator communicator)
        {
            _communicator = communicator;
        }

        public void Handle(IEvent @event)
        {
            _communicator.Communicate(_phraseBook.GetRandomPhrase());
        }
    }
}
