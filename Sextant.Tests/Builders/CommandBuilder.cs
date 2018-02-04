// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Sextant.Tests.Builders
{
    public class CommandBuilder
    {
        private string _command = Guid.NewGuid().ToString();

        public static implicit operator TestCommand(CommandBuilder b) => new TestCommand(b._command);

        public List<TestCommand> InAList() => new List<TestCommand> { this };       
    }
}
