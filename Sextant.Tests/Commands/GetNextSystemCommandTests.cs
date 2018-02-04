// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Sextant.Domain;
using Sextant.Domain.Commands;
using Sextant.Domain.Entities;
using Sextant.Domain.Phrases;
using Sextant.Tests.Builders;
using Xunit;
using FluentAssertions;

namespace Sextant.Tests.Commands
{
    public class GetNextSystemCommandTests : CommandTestBase
    {
        [Fact]
        public void GetNextSystem_Communicates_NextSystem()
        {
            TestCommunicator communicator = CreateCommunicator();
            Navigator navigator           = CreateNavigator();
            GetNextSystemPhrases phrases  = new GetNextSystemPhrases { Phrases = new[] { "{0}" } };
            GetNextSystemCommand sut      = new GetNextSystemCommand(communicator, navigator, phrases);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand);

            Celestial celestial = Build.A.Celestial.ThatHasNotBeenScanned();
            StarSystem system   = Build.A.StarSystem.WithCelestial(celestial);
            navigator.PlanExpedition(new[] { system });

            sut.Handle(testEvent);

            communicator.MessagesCommunicated.Single().Should().Be(system.Name);
        }
    }
}
