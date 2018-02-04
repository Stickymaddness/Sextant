// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Events;

namespace Sextant.Domain.Commands
{
    public class ClearComsCommand : ICommand
    {
        private readonly ICommunicator _communicator;

        public ClearComsCommand(ICommunicator communicator)
        {
            _communicator = communicator;
        }

        public string SupportedCommand     => "clear-coms";
        public bool Handles(IEvent @event) => @event.Event == "clear-coms";
        public void Handle(IEvent @event)  => _communicator.StopComminicating();
    }
}
