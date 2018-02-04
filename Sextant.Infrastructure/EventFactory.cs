// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Infrastructure.Journal;

namespace Sextant.Infrastructure
{
    public static class EventFactory
    {
        internal static JournalEvent FromJournal(JournalEntry journalEntry)
        {
            return new JournalEvent(journalEntry.Event, journalEntry.Entry);
        }

        public static VoiceAttackEvent FromVoiceAttack(string context)
        {
            return new VoiceAttackEvent(context);
        }
    }
}
