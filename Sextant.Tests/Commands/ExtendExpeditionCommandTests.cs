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
    public class ExtendExpeditionCommandTests : CommandTestBase
    {
        private readonly PlanExpeditionCommand _sut;
        private readonly Navigator _navigator;
        private readonly TestCommunicator _communicator;
        private readonly List<StarSystem> _starSystems;

        private readonly TestUserDataService _userDataService;

        public ExtendExpeditionCommandTests()
        {
            Celestial celestial = Build.A.Celestial.ThatHasNotBeenScanned();
            _starSystems                  = Build.A.StarSystem.WithCelestial(celestial).InAList();
            _userDataService              = new TestUserDataService(_starSystems);
            _navigator                    = CreateNavigator(new MemoryDataStore<StarSystemDocument>());
            _communicator                 = CreateCommunicator();
            _sut                          = new ExtendExpeditionCommand(_communicator, _navigator, _userDataService, BuildPhrases());
        }

        private PlotExpeditionPhrases BuildPhrases() => new PlotExpeditionPhrases
        {
            ExpeditionPlotted = "{0}{1}Plotted",
            UnableToPlot = "UnableToPlot",            
        };

        [Fact]
        public void ExtendExpedition_With_Invalid_Data_Handles_Gracefully()
        {
            _userDataService.StarSystems = null;

            TestEvent testEvent          = Build.An.Event.WithEvent(_sut.SupportedCommand);

            _navigator.GetAllExpeditionSystems().Should().BeEmpty();

            _sut.Handle(testEvent);

            _navigator.GetAllExpeditionSystems().Should().BeEmpty();
            _navigator.ExpeditionStarted.Should().BeFalse();
            _communicator.MessagesCommunicated.Single().Should().Contain("UnableToPlot");
        }

        [Fact]
        public void ExtendExpedition_Without_Existing_Expedition_Does_Not_Plot()
        {
            //Todo
        }

        [Fact]
        public void ExtendExpedition_With_Existing_Expedition_Extends_Expedition()
        {
            Celestial celestial              = Build.A.Celestial.ThatHasNotBeenScanned();
            List<StarSystem> existingSystems = Build.A.StarSystem.WithCelestial(celestial).InAList();

            _navigator.PlanExpedition(existingSystems);

            TestEvent testEvent = Build.An.Event.WithEvent(_sut.SupportedCommand);

            _sut.Handle(testEvent);

            var allSystems = new List<StarSystem>();
            allSystems.AddRange(_starSystems);
            allSystems.AddRange(existingSystems);

            _navigator.GetAllExpeditionSystems().ShouldBeEquivalentTo(allSystems);
            _communicator.MessagesCommunicated.Single().Should().Contain("Plotted");
        }
    }
}
