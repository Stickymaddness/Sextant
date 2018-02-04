// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class InitializedCommand : ResponseCommandBase
    {
        internal override PhraseBook _phraseBook { get; }

        internal override string _supportedCommand => "initialized";

        public InitializedCommand(ICommunicator communicator, InitializedPhrases phrases)
            : base(communicator)
        {
            _phraseBook = PhraseBook.Ingest(phrases.Phrases);
        }
    }
}