// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Sextant.Infrastructure.Journal
{
    public class JournalEntry
    {
        public Dictionary<string, object> Entry { get; private set; }

        public string Event => Entry?[JournalFields.Event].ToString();
        public bool IsValid => string.IsNullOrWhiteSpace(Event) == false && Entry != null && Entry.ContainsKey("timestamp");

        public static JournalEntry InvalidEntry => new JournalEntry();

        private JournalEntry()
        { }

        public JournalEntry(Dictionary<string, object> journalObject)
        {
            if (journalObject == null)
                return;

            Entry = journalObject;
        }
    }
}
