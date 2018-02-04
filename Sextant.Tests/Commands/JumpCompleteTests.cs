// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Commands;
using Sextant.Infrastructure.Repository;
using Sextant.Tests.Builders;
using Xunit;
using FluentAssertions;

namespace Sextant.Tests.Commands
{
    public class JumpCompleteTests : CommandTestBase
    {
        [Fact]
        public void JumpComplete_Updates_Location_And_Fuel()
        {
            PlayerStatusRepository playerRepository = CreatePlayerStatusRepository();
            JumpCompleteCommand sut                 = new JumpCompleteCommand(playerRepository);

            TestEvent testEvent = Build.An.Event.WithEvent(sut.SupportedCommand)
                                                .WithPayload("StarSystem", "Test")
                                                .WithPayload("FuelLevel", 1.0);

            sut.Handle(testEvent);

            playerRepository.Location.Should().Be("Test");
            playerRepository.FuelCapacity.Should().Be(1.0);
        }
    }
}
