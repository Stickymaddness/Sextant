// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using System.Collections.Generic;

namespace Sextant.Tests
{
    public class TestCommunicator : ICommunicator
    {
        public List<string> MessagesCommunicated { get; } = new List<string>();

        public void Communicate(string message)
        {
            MessagesCommunicated.Add(message);
        }

        public void Initialize()
        { }

        public void StopComminicating()
        { }
    }
}
