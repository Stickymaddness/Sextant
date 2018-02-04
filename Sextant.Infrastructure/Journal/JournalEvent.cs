// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Events;
using System.Collections.Generic;

namespace Sextant.Infrastructure.Journal
{
    public class JournalEvent : IEvent
    {
        public string Event { get; private set; }
        public Dictionary<string, object> Payload { get; }

        public JournalEvent(string command, Dictionary<string, object> payload)
        {
            Event   = command;
            Payload = payload;
        }
    }
}
