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
    public class PlanExpeditionCommandTests : CommandTestBase
    {
        private readonly PlanExpeditionCommand _sut;
        private readonly Navigator _navigator;
        private readonly TestCommunicator _communicator;
        private readonly List<StarSystem> _starSystems;

        private readonly TestUserDataService _userDataService;

        public PlanExpeditionCommandTests()
        {
            Celestial celestial = Build.A.Celestial.ThatHasNotBeenScanned();
            _starSystems        = Build.A.StarSystem.WithCelestial(celestial).InAList();

            _userDataService    = new TestUserDataService(_starSystems);
            _navigator          = CreateNavigator(new MemoryDataStore<StarSystemDocument>());
            _communicator       = CreateCommunicator();
            _sut                = new PlanExpeditionCommand(_communicator, _navigator, _userDataService, BuildPhrases());
        }

        private PlotExpeditionPhrases BuildPhrases() => new PlotExpeditionPhrases
        {
            ExpeditionPlotted = "{0}{1}Plotted",
            ExpeditionExists  = "ExpeditionExists",
            UnableToPlot      = "UnableToPlot"
        };

        [Fact]
        public void PlotExpedition_With_Conditions_Passing_Plots_Expedition_And_Communicates()
        {
            TestEvent testEvent = Build.An.Event.WithEvent(_sut.SupportedCommand);

            _navigator.GetAllExpeditionSystems().Should().BeEmpty();

            _sut.Handle(testEvent);

            _navigator.GetAllExpeditionSystems().ShouldBeEquivalentTo(_starSystems);
            _communicator.MessagesCommunicated.Single().Should().Contain("Plotted");
        }

        [Fact]
        public void PlotExpedition_With_Existing_Expedition_Does_Not_Plot()
        {
            Celestial celestial              = Build.A.Celestial.ThatHasNotBeenScanned();
            List<StarSystem> existingSystems = Build.A.StarSystem.WithCelestial(celestial).InAList();

            _navigator.PlanExpedition(existingSystems);

            TestEvent testEvent = Build.An.Event.WithEvent(_sut.SupportedCommand);

            _sut.Handle(testEvent);

            _navigator.GetAllExpeditionSystems().ShouldBeEquivalentTo(existingSystems);
            _communicator.MessagesCommunicated.Single().Should().Be("ExpeditionExists");
        }

        [Fact]
        public void PlotExpedition_With_Invalid_Data_Handles_Gracefully()
        {
            _userDataService.StarSystems = null;

            TestEvent testEvent = Build.An.Event.WithEvent(_sut.SupportedCommand);

            _navigator.GetAllExpeditionSystems().Should().BeEmpty();

            _sut.Handle(testEvent);

            _navigator.GetAllExpeditionSystems().Should().BeEmpty();
            _navigator.ExpeditionStarted.Should().BeFalse();
            _communicator.MessagesCommunicated.Single().Should().Contain("UnableToPlot");
        }
    }
}
