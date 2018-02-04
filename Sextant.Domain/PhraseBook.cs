// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sextant.Domain
{
    internal class PhraseBook
    {
        private List<string> _phrases = new List<string>();

        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());

        internal static PhraseBook Ingest(IEnumerable<string> phrases)
        {
            return new PhraseBook(phrases);
        }

        public PhraseBook(IEnumerable<string> phrases)
        {
            _phrases = phrases.ToList();
        }

        internal string GetRandomPhrase()
        {
            int index = _random.Next(0, _phrases.Count());

            return _phrases[index];
        }

        internal string GetRandomPhraseWith(params object[] args)
        {
            return string.Format(GetRandomPhrase(), args);
        }
    }
}
