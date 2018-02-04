// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Events;
using System.Collections.Generic;

namespace Sextant.Tests
{
    public class TestEvent : IEvent
    {
        public string Event { get; set; }
        public Dictionary<string, object> Payload { get; }

        public TestEvent(string @event)
        {
            Event = @event;
        }

        public TestEvent(string @event, Dictionary<string, object> payload)
        {
            Event   = @event;
            Payload = payload;
        }
    }
}
