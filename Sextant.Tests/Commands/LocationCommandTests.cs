// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Sextant.Domain.Commands;
using Sextant.Infrastructure.Repository;
using Sextant.Tests;
using Sextant.Tests.Builders;
using Xunit;

namespace Sextant.Tests.Commands
{
    public class LocationCommandTests : CommandTestBase
    {
        private LocationCommand CreateSut(PlayerStatusRepository playerStatusRepository) => new LocationCommand(playerStatusRepository);

        [Fact]
        public void LocationCommand_Should_UpdateLocation()
        {
            PlayerStatusRepository repository = CreatePlayerStatusRepository();
            LocationCommand sut               = CreateSut(repository);

            repository.SetLocation(Build.A.StarSystem.Name);

            string expectedSystem = Build.A.StarSystem.Name;

            TestEvent loadEvent = Build.An.Event.WithEvent("LoadGame").WithPayload("StarSystem", expectedSystem);

            sut.Handle(loadEvent);

            repository.Location.Should().Be(expectedSystem);
        }
    }
}
