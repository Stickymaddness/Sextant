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
    public class FindNextSystemCommandTests : CommandTestBase
    {
        [Fact]
        public void GetNextSystem_Communicates_NextSystem()
        {
            TestCommunicator communicator = CreateCommunicator();
            Navigator navigator           = CreateNavigator();
            TestGalaxyMap galaxyMap       = new TestGalaxyMap();
            FindNextSystemCommand sut     = new FindNextSystemCommand(communicator, navigator, galaxyMap, TestPhraseBuilder.Build<FindNextSystemPhrases>());

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand);

            Celestial celestial = Build.A.Celestial.ThatHasNotBeenScanned();
            StarSystem system   = Build.A.StarSystem.WithCelestial(celestial);
            navigator.PlanExpedition(new[] { system });

            sut.Handle(testEvent);

            galaxyMap.Systems.Single().Should().Be(system.Name);
        }
    }
}
