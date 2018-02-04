// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Commands;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Sextant.Tests.Builders;

namespace Sextant.Domain.Tests
{
    public class CommandRegistryTests
    {
        [Fact]
        public void CommandRegistry_WithCommands_SupportsThoseCommands()
        {
            List<TestCommand> commands = Build.Many.Commands(Build.A.Command, Build.A.Command, Build.A.Command);
            CommandRegistry sut        = new CommandRegistry(commands);

            commands.All(c => sut.Supports(c.SupportedCommand)).Should().BeTrue();
        }

        [Fact]
        public void CommandRegistry_WithACommand_DoesNotSupportUnknownCommand()
        {
            List<TestCommand> commands = Build.A.Command.InAList();
            CommandRegistry sut        = new CommandRegistry(commands);
            TestCommand unknownCommand = Build.A.Command;

            sut.Supports(unknownCommand.SupportedCommand).Should().BeFalse();
        }
    }
}
