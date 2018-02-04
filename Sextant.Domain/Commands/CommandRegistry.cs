// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Sextant.Domain.Commands
{
    public class CommandRegistry : ICommandRegistry
    {
        private List<string> _supportedCommands;

        public CommandRegistry(IEnumerable<ICommand> commands)
        {
            _supportedCommands = commands.Select(c => c.SupportedCommand.ToUpper()).ToList();
        }

        public bool Supports(string command)
        {
            return _supportedCommands.Contains(command.ToUpper());
        }
    }
}
