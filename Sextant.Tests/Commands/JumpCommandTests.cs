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
    public class JumpCommandTests : CommandTestBase
    {
        private readonly Navigator _navigator;
        private readonly TestCommunicator _communicator;
        private readonly List<StarSystem> _starSystems;

        private readonly TestUserDataService _userDataService;
        private readonly JumpPhrases _phrases = TestPhraseBuilder.Build<JumpPhrases>();

        public JumpCommandTests()
        {
            Celestial celestial  = Build.A.Celestial.ThatHasNotBeenScanned();
            Celestial celestial2 = Build.A.Celestial.ThatHasNotBeenScanned();
            _starSystems         = Build.A.StarSystem.WithCelestials(celestial, celestial, celestial2).InAList();
            _userDataService     = new TestUserDataService(_starSystems);
            _navigator           = CreateNavigator(new MemoryDataStore<StarSystemDocument>());
            _communicator        = CreateCommunicator();
        }

        [Fact]
        public void JumpCommand_With_Supercruise_Payload_Does_Nothing()
        {
            var sut = new JumpCommand(_communicator, _navigator, _phrases, new Preferences());

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand).WithPayload("JumpType", "Supercruise");

            sut.Handle(testEvent);

            _communicator.MessagesCommunicated.Should().BeEmpty();
        }

        [Fact]
        public void JumpCommand_With_System_Not_In_Expedition_Skips_System_And_Communicates()
        {
            var preferences     = new Preferences() { CommunicateSkippableSystems = true };
            var sut             = new JumpCommand(_communicator, _navigator, _phrases, preferences);
            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand).WithPayload("StarSystem", "Test");

            sut.Handle(testEvent);

            _communicator.MessagesCommunicated[0].Should().Be(_phrases.Jumping.Single());
            _communicator.MessagesCommunicated[1].Should().Be(_phrases.Skipping.Single());
        }

        [Fact]
        public void JumpCommand_With_System_Not_In_Expedition_Skips_System_And_Does_Not_Communicate()
        {
            var preferences = new Preferences() { CommunicateSkippableSystems = false };
            var sut = new JumpCommand(_communicator, _navigator, _phrases, preferences);
            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand).WithPayload("StarSystem", "Test");

            sut.Handle(testEvent);

            _communicator.MessagesCommunicated.Single().Should().Be(_phrases.Jumping.Single());
        }

        [Fact]
        public void JumpCommand_With_System_Already_Scanned_Skips_System_And_Communicates()
        {
            var preferences = new Preferences() { CommunicateSkippableSystems = true };
            var sut         = new JumpCommand(_communicator, _navigator, _phrases, preferences);

            var celestial   = Build.A.Celestial.ThatHasBeenScanned();
            var system      = Build.A.StarSystem.WithCelestial(celestial);
            var systems     = Build.Many.StarSystems(system);

            _navigator.ExtendExpedition(systems);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand).WithPayload("StarSystem", system.Name);

            sut.Handle(testEvent);

            _communicator.MessagesCommunicated[0].Should().Be(_phrases.Jumping.Single());
            _communicator.MessagesCommunicated[1].Should().Be(_phrases.AlreadyScanned.Single());
        }

        [Fact]
        public void JumpCommand_With_Already_Scanned_Skips_System_And_Does_Not_Communicate()
        {
            var preferences = new Preferences() { CommunicateSkippableSystems = false };
            var sut         = new JumpCommand(_communicator, _navigator, _phrases, preferences);

            var celestial   = Build.A.Celestial.ThatHasBeenScanned();
            var system      = Build.A.StarSystem.WithCelestial(celestial);
            var systems     = Build.Many.StarSystems(system);

            _navigator.ExtendExpedition(systems);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand).WithPayload("StarSystem", system.Name);

            sut.Handle(testEvent);

            _communicator.MessagesCommunicated.Single().Should().Be(_phrases.Jumping.Single());
        }

        [Fact]
        public void JumpCommand_With_Unscanned_System_In_Expedition_Communicates()
        {
            var sut = new JumpCommand(_communicator, _navigator, _phrases, new Preferences());

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand).WithPayload("StarSystem", _starSystems.First().Name);

            _navigator.ExtendExpedition(_starSystems);

            sut.Handle(testEvent);

            _communicator.MessagesCommunicated[0].Should().Be(_phrases.Jumping.Single());
            _communicator.MessagesCommunicated[1].Should().Contain(_phrases.Scanning.Single());
        }
    }
}
