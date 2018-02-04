// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain.Phrases;

namespace Sextant.Domain.Commands
{
    public class LaunchSRVCommand : ResponseCommandBase
    {
        internal override string _supportedCommand => "LaunchSRV";

        internal override PhraseBook _phraseBook { get; }

        public LaunchSRVCommand(ICommunicator communicator, LaunchSRVPhrases phrases) 
            : base(communicator)
        {
            _phraseBook = PhraseBook.Ingest(phrases.Phrases);
        }
    }
}
