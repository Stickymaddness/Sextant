// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Collections.Generic;
using Sextant.Domain;
using Sextant.Domain.Commands;
using Sextant.Domain.Entities;
using Sextant.Domain.Phrases;
using Sextant.Infrastructure.Repository;
using Sextant.Tests.Builders;
using Xunit;
using FluentAssertions;

namespace Sextant.Tests.Commands
{
    public class NextScanCommandTests : CommandTestBase
    {
        private readonly Navigator              _navigator;
        private readonly TestEvent              _testEvent;
        private readonly NextScanCommand        _sut;
        private readonly NextScanPhrases        _phrases;
        private readonly TestCommunicator       _communicator;
        private readonly PlayerStatusRepository _playerStatus;

        public NextScanCommandTests()
        {
            _phrases      = TestPhraseBuilder.Build<NextScanPhrases>();
            _communicator = CreateCommunicator();
            _playerStatus = CreatePlayerStatusRepository();
            _navigator    = CreateNavigator();

            _sut          = new NextScanCommand(_communicator, _navigator, _playerStatus, _phrases);
            _testEvent    = Build.An.Event.WithEvent(_sut.SupportedCommand);
        }

        [Fact]
        public void NextScan_With_System_Not_In_Expedition_Communicates_Skip_Phrase()
        {
            _sut.Handle(_testEvent);
            _communicator.MessagesCommunicated.Single().Should().Be(_phrases.SkipSystem.Single());
        }

        [Fact]
        public void NextScan_With_System_In_Expedition_But_Everything_Scanned_Communicates_Complete_Phrase()
        {
            Celestial celestial      = Build.A.Celestial.ThatHasBeenScanned();
            List<StarSystem> systems = Build.A.StarSystem.WithCelestial(celestial).InAList();

            _playerStatus.SetLocation(systems.Single().Name);

            _navigator.PlanExpedition(systems);

            _sut.Handle(_testEvent);
            _communicator.MessagesCommunicated.Single().Should().Be(_phrases.ScansComplete.Single());
        }

        [Fact]
        public void NextScan_With_System_In_Expedition_With_Unscanned_Celestial_Communicates_Scan_Phrase()
        {
            Celestial celestial      = Build.A.Celestial.ThatHasNotBeenScanned();
            List<StarSystem> systems = Build.A.StarSystem.WithCelestial(celestial).InAList();

            _playerStatus.SetLocation(systems.Single().Name);

            _navigator.PlanExpedition(systems);

            _sut.Handle(_testEvent);
            _communicator.MessagesCommunicated.Single().Should().Be(_phrases.NextScan.Single());
        }
    }
}
