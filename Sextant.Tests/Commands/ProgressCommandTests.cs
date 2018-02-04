// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using Sextant.Domain.Commands;
using Sextant.Domain.Phrases;
using Sextant.Infrastructure.Repository;
using Sextant.Tests.Builders;
using Xunit;
using FluentAssertions;
using Sextant.Domain.Entities;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Sextant.Tests.Commands
{
    public class ProgressCommandTests : CommandTestBase
    {
        [Fact]
        public void ProgressCommand_Communicates_Progress()
        {
            TestCommunicator communicator       = CreateCommunicator();
            Navigator navigator                 = CreateNavigator();
            PlayerStatusRepository playerStatus = CreatePlayerStatusRepository();
            ProgressPhrases phrases             = TestPhraseBuilder.Build<ProgressPhrases>();

            ProgressCommand sut                 = new ProgressCommand(communicator, navigator, playerStatus, phrases);

            Celestial celestial                 = Build.A.Celestial;
            List<StarSystem> systems            = Build.A.StarSystem.WithCelestials(celestial).InAList();

            navigator.PlanExpedition(systems);
            playerStatus.SetExpeditionStart(DateTimeOffset.Now);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand);

            sut.Handle(testEvent);

            communicator.MessagesCommunicated[0].Should().Be(phrases.Progress.Single());
            communicator.MessagesCommunicated[1].Should().Be(phrases.SystemsScanned.Single());
        }
    }
}
