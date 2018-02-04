// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Collections.Generic;
using Sextant.Domain.Entities;

namespace Sextant.Tests.Builders
{
    public static class Build
    {
        public static class A
        {
            public static CelestialBuilder Celestial   => new CelestialBuilder();
            public static CommandBuilder Command       => new CommandBuilder();
            public static StarSystemBuilder StarSystem => new StarSystemBuilder();
        }

        public static class An
        {
            public static EventBuilder Event => new EventBuilder();
        }

        public static class Many
        {
            public static List<Celestial> Celestials                                => new List<Celestial>();
            public static List<StarSystem> StarSystems(params StarSystem[] systems) => systems.ToList();
            public static List<TestCommand> Commands(params TestCommand[] commands) => commands.ToList();
        }

        public static List<Celestial> With(this List<Celestial> list, params Celestial[] celestials)
        {
            list.AddRange(celestials);
            return list;
        }
    }
}
