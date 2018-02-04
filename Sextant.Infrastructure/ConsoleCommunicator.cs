// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using System;

namespace Sextant.Infrastructure
{
    public class ConsoleCommunicator : ICommunicator
    {
        public void Communicate(string message) => Console.WriteLine(message);
        public void StopComminicating() { }
        public void Initialize() { }
    }
}
