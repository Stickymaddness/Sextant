// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Collections.Generic;
using Sextant.Domain.Events;

namespace Sextant.Domain.Commands
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly IEnumerable<ICommand> _commands;

        public CommandExecutor(IEnumerable<ICommand> commands)
        {
            _commands = commands.ToList();
        }

        public void Handle(IEvent @event)
        {
            foreach (var command in _commands)
                if (command.Handles(@event))
                    command.Handle(@event);
        }
    }
}
