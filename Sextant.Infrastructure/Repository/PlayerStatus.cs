// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Sextant.Infrastructure.Repository
{
    public class PlayerStatus
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public double FuelCapacity { get; set; }
        public DateTimeOffset ExpeditionStart { get; set; }

        public PlayerStatus()
        { }

        public PlayerStatus(int id)
        {
            Id = id;
        }
    }
}
