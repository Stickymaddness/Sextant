// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using Sextant.Domain.Commands;
using Sextant.Domain.Entities;
using Sextant.Domain.Phrases;
using Sextant.Infrastructure.Repository;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using Sextant.Tests.Builders;

namespace Sextant.Tests.Commands
{
    public class CancelExpeditionCommandTests : CommandTestBase
    {
        private readonly Navigator _navigator;
        private readonly CancelExpeditionCommand _sut;

        public CancelExpeditionCommandTests()
        {
            TestCommunicator communicator = CreateCommunicator();
            _navigator                    = CreateNavigator(new MemoryDataStore<StarSystemDocument>());
            _sut                          = new CancelExpeditionCommand(communicator, _navigator, CreatePhrases<CancelExpeditionPhrases>());
        }

        [Fact]
        public void CancelExpedition_Supports_Event()
        {
            TestEvent testEvent = Build.An.Event.WithEvent(_sut.SupportedCommand);

            _sut.Handles(testEvent).Should().BeTrue();
        }

        [Fact]
        public void CancelExpedition_CancelsExpedition_Only_After_Confirmation()
        {
            Celestial celestial          = Build.A.Celestial.ThatHasNotBeenScanned();
            StarSystem firstSystem       = Build.A.StarSystem.WithCelestial(celestial);
            StarSystem secondSystem      = Build.A.StarSystem.WithCelestial(celestial);
            List<StarSystem> starSystems = Build.Many.StarSystems(firstSystem, secondSystem);

            _navigator.PlanExpedition(starSystems);

            TestEvent testEvent = Build.An.Event.WithEvent(_sut.SupportedCommand);

            _sut.Handle(testEvent);

            _navigator.ExpeditionStarted.Should().BeTrue();
            _navigator.ExpeditionComplete.Should().BeFalse();
            _navigator.CelestialsRemaining().Should().Be(2);
            _navigator.SystemsRemaining().Should().Be(2);

            _sut.Handle(testEvent);

            _navigator.ExpeditionStarted.Should().BeFalse();
            _navigator.ExpeditionComplete.Should().BeTrue();
            _navigator.CelestialsRemaining().Should().Be(0);
            _navigator.SystemsRemaining().Should().Be(0);
        }
    }
}