// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using Sextant.Domain.Commands;
using Sextant.Domain.Events;
using Sextant.Tests;
using System.Collections.Generic;
using Xunit;

namespace Sextant.Domain.Tests
{
    public class CommandExecutorTests
    {
        [Fact]
        public void CommandExecutor_GivenSupportCommand_ExecutesCommand()
        {
            const string commandString = "Test";

            var command = new CommandStub(commandString);

            var sut    = new CommandExecutor(new List<ICommand> { new CommandStub(commandString) });
            var @event = new TestEvent(commandString);

            sut.Handle(@event);

            CommandStub.HandledEvents.Should().Contain(@event);
        }

        private class CommandStub : ICommand
        {
            public static List<IEvent> HandledEvents = new List<IEvent>();

            public void Handle(IEvent @event)
            {
                HandledEvents.Add(@event);
            }

            public bool Handles(IEvent @event) => @event.Event == _supportedCommand;
            public string SupportedCommand => _supportedCommand;

            private string _supportedCommand;

            public CommandStub(string command)
            {
                _supportedCommand = command;
            }
        }
    }
}
