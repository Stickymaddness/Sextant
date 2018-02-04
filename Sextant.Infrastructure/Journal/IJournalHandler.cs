// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Sextant.Infrastructure.Journal
{
    public interface IJournalHandler
    {
        void Handle(List<string> journalLines);
    }
}