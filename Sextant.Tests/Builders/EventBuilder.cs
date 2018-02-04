// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Sextant.Tests.Builders
{
    public class EventBuilder
    {
        private string _event;
        private Dictionary<string, object> _payload = new Dictionary<string, object>();

        public static implicit operator TestEvent(EventBuilder b) => new TestEvent(b._event, b._payload);

        public EventBuilder WithEvent(string @event)
        {
            _event = @event;
            return this;
        }

        public EventBuilder WithPayload(string key, object value)
        {
            _payload[key] = value;
            return this;
        }
    }
}
