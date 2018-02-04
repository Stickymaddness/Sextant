// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Events;
using System.Collections.Generic;

namespace Sextant.Infrastructure
{
    public class VoiceAttackEvent : IEvent
    {
        public string Event { get; private set; }
        public Dictionary<string, object> Payload { get; }

        public VoiceAttackEvent(string command)
        {
            Event   = command;
            Payload = null;
        }
    }
}
