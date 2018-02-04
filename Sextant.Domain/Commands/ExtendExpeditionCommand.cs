// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Entities;
using Sextant.Domain.Events;
using Sextant.Domain.Phrases;
using System.Collections.Generic;

namespace Sextant.Domain.Commands
{
    public class ExtendExpeditionCommand : PlanExpeditionCommand
    {
        public override string SupportedCommand => "extend_expedition";

        public override bool Handles(IEvent @event) => @event.Event == SupportedCommand;

        public ExtendExpeditionCommand(ICommunicator communicator, INavigator navigator, IUserDataService userDataService, PlotExpeditionPhrases phrases)
            : base(communicator, navigator, userDataService, phrases)
        { }

        public override void Handle(IEvent @event)
        {
            IEnumerable<StarSystem> expeditionData = _userDataService.GetExpeditionData();

            bool success = _navigator.ExtendExpedition(expeditionData);

            if (!success)
            {
                _communicator.Communicate(_unableToPlot);
                return;
            }

            CommunicateExpedition();
        }
    }
}
