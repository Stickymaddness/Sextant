// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Sextant.Infrastructure.Journal
{
    internal static class JournalParser
    {
        internal static JournalEntry Parse(string journalLine)
        {
            try
            {
                var journalObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(journalLine);

                return new JournalEntry(journalObject);

            }
            catch (Exception)
            {
                return JournalEntry.InvalidEntry;
            }
        }
    }
}
