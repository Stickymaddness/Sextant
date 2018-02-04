// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using Sextant.Domain.Commands;
using Sextant.Domain.Entities;
using Sextant.Domain.Phrases;
using Sextant.Infrastructure.Repository;
using Sextant.Tests.Builders;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace Sextant.Tests.Commands
{
    public class SkipSystemCommandTests : CommandTestBase
    {
        [Fact]
        public void SkipSystem_With_System_In_Expedition_Marks_System_As_Scanned()
        {
            TestCommunicator communicator       = CreateCommunicator();
            Navigator navigator                 = CreateNavigator();
            PlayerStatusRepository playerStatus = CreatePlayerStatusRepository();
            var skipSystemPhrases               = TestPhraseBuilder.Build<SkipSystemPhrases>();
            var scansRemainingPhrases           = TestPhraseBuilder.Build<ScansRemainingPhrases>();

            SkipSystemCommand sut               = new SkipSystemCommand(communicator, navigator, playerStatus, skipSystemPhrases, scansRemainingPhrases);

            Celestial celestial                 = Build.A.Celestial.ThatHasNotBeenScanned();
            StarSystem currentSystem            = Build.A.StarSystem.WithCelestials(celestial, celestial, celestial);
            StarSystem nextSystem               = Build.A.StarSystem.WithCelestials(celestial, celestial, celestial);
            List<StarSystem> systems            = Build.Many.StarSystems(currentSystem, nextSystem);

            navigator.PlanExpedition(systems);
            playerStatus.SetLocation(currentSystem.Name);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand);

            sut.Handle(testEvent);

            StarSystem storedSystem = navigator.GetSystem(currentSystem.Name);

            storedSystem.Scanned.Should().BeTrue();
            storedSystem.Celestials.All(c => c.Scanned).Should().BeTrue();

            navigator.GetNextSystem().ShouldBeEquivalentTo(nextSystem);
        }

        [Fact]
        public void SkipSystem_With_System_Not_In_Expedition_Does_Nothing()
        {
            TestCommunicator communicator       = CreateCommunicator();
            Navigator navigator                 = CreateNavigator();
            PlayerStatusRepository playerStatus = CreatePlayerStatusRepository();
            var skipSystemPhrases               = TestPhraseBuilder.Build<SkipSystemPhrases>();
            var scansRemainingPhrases           = TestPhraseBuilder.Build<ScansRemainingPhrases>();

            SkipSystemCommand sut               = new SkipSystemCommand(communicator, navigator, playerStatus, skipSystemPhrases, scansRemainingPhrases);

            Celestial celestial                 = Build.A.Celestial.ThatHasNotBeenScanned();
            StarSystem firstSystem              = Build.A.StarSystem.WithCelestials(celestial, celestial, celestial);
            StarSystem nextSystem               = Build.A.StarSystem.WithCelestials(celestial, celestial, celestial);
            List<StarSystem> systems            = Build.Many.StarSystems(firstSystem, nextSystem);

            navigator.PlanExpedition(systems);
            playerStatus.SetLocation("Test");

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand);

            sut.Handle(testEvent);

            List<StarSystem> storedSystems = navigator.GetAllExpeditionSystems();

            storedSystems.All(s => s.Scanned == false).Should().BeTrue();
            storedSystems.SelectMany(s => s.Celestials).All(s => s.Scanned == false).Should().BeTrue();

            navigator.GetNextSystem().ShouldBeEquivalentTo(firstSystem);
        }
    }
}
