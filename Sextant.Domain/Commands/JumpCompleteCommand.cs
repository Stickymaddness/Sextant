// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Events;

namespace Sextant.Domain.Commands
{
    public class JumpCompleteCommand : ICommand
    {
        public string SupportedCommand => "FSDJump";
        public bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        private readonly IPlayerStatus _playerStatus;

        public JumpCompleteCommand(IPlayerStatus playerStatus)
        {
            _playerStatus = playerStatus;
        }

        public void Handle(IEvent @event)
        {
            string location  = @event.Payload["StarSystem"].ToString();
            double fuelLevel = (double)@event.Payload["FuelLevel"];

            _playerStatus.SetLocation(location);
            _playerStatus.SetFuelCapacity(fuelLevel);
        }
    }
}
