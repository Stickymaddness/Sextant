// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Collections.Generic;
using Sextant.Domain;
using Sextant.Domain.Commands;
using Sextant.Domain.Entities;
using Sextant.Infrastructure.Repository;
using Sextant.Tests.Builders;
using Sextant.Domain.Phrases;
using Xunit;
using FluentAssertions;

namespace Sextant.Tests.Commands
{
    public class ScansRemainingCommandTests : CommandTestBase
    {
        private readonly TestCommunicator       _communicator;
        private readonly Navigator              _navigator;
        private readonly PlayerStatusRepository _playerStatus;
        private readonly ScansRemainingPhrases _scansRemainingPhrases;
        private readonly ScansRemainingCommand _sut;

        public ScansRemainingCommandTests()
        {
            _communicator          = CreateCommunicator();
            _navigator             = CreateNavigator();
            _playerStatus          = CreatePlayerStatusRepository();
            _scansRemainingPhrases = TestPhraseBuilder.Build<ScansRemainingPhrases>();
            _sut                   = new ScansRemainingCommand(_communicator, _navigator, _playerStatus, _scansRemainingPhrases);
        }

        [Fact]
        public void ScansRemaining_With_System_Not_In_Expedition_Communicates_Skip_Phrase()
        {
            TestEvent testEvent = Build.An.Event.WithEvent(_sut.SupportedCommand);

            _sut.Handle(testEvent);

            _communicator.MessagesCommunicated.Single().Should().Be(_scansRemainingPhrases.SkipSystem.Single());
        }

        [Fact]
        public void ScansRemaining_With_System_In_Expedition_But_No_Scans_Remaining_Communicates_Skip_Phrase()
        {
            Celestial celestial      = Build.A.Celestial.ThatHasBeenScanned();
            List<StarSystem> systems = Build.A.StarSystem.WithCelestial(celestial).InAList();

            _playerStatus.SetLocation(systems.Single().Name);

            _navigator.PlanExpedition(systems);

            TestEvent testEvent = Build.An.Event.WithEvent(_sut.SupportedCommand);

            _sut.Handle(testEvent);

            _communicator.MessagesCommunicated.Single().Should().Be(_scansRemainingPhrases.SystemComplete.Single());
        }

        [Fact]
        public void ScansRemaining_With_System_In_Expedition_Communicates_Remaining_Phrase()
        {
            Celestial celestial      = Build.A.Celestial.ThatHasNotBeenScanned();
            List<StarSystem> systems = Build.A.StarSystem.WithCelestial(celestial).InAList();

            _playerStatus.SetLocation(systems.Single().Name);

            _navigator.PlanExpedition(systems);

            TestEvent testEvent = Build.An.Event.WithEvent(_sut.SupportedCommand);

            _sut.Handle(testEvent);

            _communicator.MessagesCommunicated.Single().Should().Be(_scansRemainingPhrases.ScansRemaining.Single());
        }
    }
}
