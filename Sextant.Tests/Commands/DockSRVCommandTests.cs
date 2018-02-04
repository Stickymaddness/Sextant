// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Commands;
using Sextant.Domain.Phrases;
using Sextant.Tests.Builders;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Sextant.Tests.Commands
{
    public class DockSRVCommandTests : CommandTestBase
    {
        [Fact]
        public void GameLoad_Should_StoreFuelCapacity()
        {
            TestCommunicator communicator = CreateCommunicator();
            DockSRVPhrases phrases        = TestPhraseBuilder.Build<DockSRVPhrases>();
            DockSRVCommand sut            = new DockSRVCommand(communicator, phrases);

            TestEvent loadEvent = Build.An.Event.WithEvent("DockSRV");

            sut.Handle(loadEvent);

            communicator.MessagesCommunicated.Single().Should().Be(phrases.Phrases.Single());
        }
    }
}
