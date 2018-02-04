// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using FluentAssertions;
using Xunit;

namespace Sextant.Domain.Tests
{
    public class PhraseBookTests
    {
        [Fact]
        public void GetRandomPhrase_WithOnePhrase_ReturnsPhrase()
        {
            string testString = Guid.NewGuid().ToString();
            PhraseBook sut    = PhraseBook.Ingest(new[] { testString });

            sut.GetRandomPhrase().Should().Be(testString);
        }

        [Fact]
        public void GetRandomPhraseWith_WithOnePhrase_ReturnsPhraseWithArgs()
        {
            string testString = "TEST{0}TEST";
            string testArg    = Guid.NewGuid().ToString();
            PhraseBook sut    = PhraseBook.Ingest(new[] { testString });

            sut.GetRandomPhraseWith(testArg).Should().Be(string.Format("TEST{0}TEST", testArg));
        }
    }
}
