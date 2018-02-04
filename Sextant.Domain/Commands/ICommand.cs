﻿// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Events;

namespace Sextant.Domain.Commands
{
    public interface ICommand
    {
        bool Handles(IEvent @event);
        void Handle(IEvent @event);
        string SupportedCommand { get; }
    } 
}