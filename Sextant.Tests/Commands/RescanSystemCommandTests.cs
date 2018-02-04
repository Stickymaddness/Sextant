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
    public class RescanSystemCommandTests : CommandTestBase
    {
        [Fact]
        public void RescanSystem_With_System_In_Expedition_Marks_System_As_Unscanned()
        {
            TestCommunicator communicator       = CreateCommunicator();
            Navigator navigator                 = CreateNavigator();
            PlayerStatusRepository playerStatus = CreatePlayerStatusRepository();
            var rescanSystemPhrases             = TestPhraseBuilder.Build<RescanSystemPhrases>();
            var scansRemainingPhrases           = TestPhraseBuilder.Build<ScansRemainingPhrases>();

            RescanSystemCommand sut             = new RescanSystemCommand(communicator, navigator, playerStatus, rescanSystemPhrases, scansRemainingPhrases);

            Celestial scannedCelestial          = Build.A.Celestial.ThatHasBeenScanned();
            Celestial unscannedCelestial        = Build.A.Celestial.ThatHasNotBeenScanned();
            StarSystem firstSystem              = Build.A.StarSystem.WithCelestials(scannedCelestial, scannedCelestial, scannedCelestial);
            StarSystem nextSystem               = Build.A.StarSystem.WithCelestials(unscannedCelestial, unscannedCelestial, unscannedCelestial);
            List<StarSystem> systems            = Build.Many.StarSystems(firstSystem);

            navigator.PlanExpedition(systems);
            playerStatus.SetLocation(firstSystem.Name);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand);

            sut.Handle(testEvent);

            List<StarSystem> storedSystems = navigator.GetAllExpeditionSystems();

            storedSystems.All(s => s.Scanned == false).Should().BeTrue();
            storedSystems.SelectMany(s => s.Celestials).All(s => s.Scanned == false).Should().BeTrue();

            navigator.GetNextSystem().Name.ShouldBeEquivalentTo(firstSystem.Name);

            communicator.MessagesCommunicated.Single().Should().Be(rescanSystemPhrases.SystemUnscanned.Single());
        }

        [Fact]
        public void RescanSystem_With_System_Not_In_Expedition_Does_Nothing()
        {
            TestCommunicator communicator       = CreateCommunicator();
            Navigator navigator                 = CreateNavigator();
            PlayerStatusRepository playerStatus = CreatePlayerStatusRepository();
            var rescanSystemPhrases             = TestPhraseBuilder.Build<RescanSystemPhrases>();
            var scansRemainingPhrases           = TestPhraseBuilder.Build<ScansRemainingPhrases>();

            RescanSystemCommand sut             = new RescanSystemCommand(communicator, navigator, playerStatus, rescanSystemPhrases, scansRemainingPhrases);

            Celestial celestial                 = Build.A.Celestial.ThatHasBeenScanned();
            List<StarSystem> systems            = Build.A.StarSystem.WithCelestial(celestial).InAList();

            navigator.PlanExpedition(systems);
            playerStatus.SetLocation("Test");

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand);

            sut.Handle(testEvent);

            navigator.GetAllExpeditionSystems().ShouldAllBeEquivalentTo(systems);

            communicator.MessagesCommunicated.Single().Should().Be(scansRemainingPhrases.SkipSystem.Single());
        }
    }
}
