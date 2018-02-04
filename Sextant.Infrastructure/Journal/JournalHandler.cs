// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Commands;
using System.Collections.Generic;

namespace Sextant.Infrastructure.Journal
{
    public class JournalHandler : IJournalHandler
    {
        private readonly ICommandRegistry _commandRegistry;
        private readonly ICommandExecutor _commandExecutor;

        public JournalHandler(ICommandRegistry commandRegistry, ICommandExecutor commandExecutor)
        {
            _commandRegistry = commandRegistry;
            _commandExecutor = commandExecutor;
        }

        public void Handle(List<string> journalLines)
        {
            foreach (var line in journalLines)
            {
                JournalEntry journalEntry = JournalParser.Parse(line);

                if (journalEntry.IsValid && _commandRegistry.Supports(journalEntry.Event))
                    _commandExecutor.Handle(EventFactory.FromJournal(journalEntry));
            }
        }
    }
}
