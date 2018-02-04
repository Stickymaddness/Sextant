// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using System.Collections.Generic;
using Sextant.Domain.Entities;

namespace Sextant.Tests
{
    public class TestUserDataService : IUserDataService
    {
        public IEnumerable<StarSystem> StarSystems { get; set; }

        public IEnumerable<StarSystem> GetExpeditionData() => StarSystems;

        public TestUserDataService(IEnumerable<StarSystem> starSystems)
        {
            StarSystems = starSystems;
        }
    }
}
