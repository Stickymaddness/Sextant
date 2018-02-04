// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Sextant.Domain
{
    public interface IPlayerStatus
    {
        string Location { get; }
        double FuelCapacity { get; }
        TimeSpan ExpeditionLength { get; }
        bool SetLocation(string location);
        bool SetFuelCapacity(double capacity);
        bool SetExpeditionStart(DateTimeOffset dateTimeStart);
    }
}
