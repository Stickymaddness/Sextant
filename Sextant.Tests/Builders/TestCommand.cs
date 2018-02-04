// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Commands;
using Sextant.Domain.Events;

namespace Sextant.Tests.Builders
{
    public class TestCommand : ICommand
    {
        private readonly string _supportedCommand;

        public string SupportedCommand => _supportedCommand;

        public TestCommand(string supportedCommand)
        {
            _supportedCommand = supportedCommand;
        }

        public void Handle(IEvent @event) { }
        public bool Handles(IEvent @event) => @event.Event == _supportedCommand;
    }
}
