// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using Sextant.Infrastructure.Settings;
using System.Speech.Synthesis;

namespace Sextant.Infrastructure.TextToSpeech
{
    public class VoiceCommunicator : ICommunicator
    {
        private SpeechSynthesizer _speechSynth;
        private readonly string _voice;

        public VoiceCommunicator(VoiceCommunicatorSettings settings)
        {
            _voice = settings.Voice;
        }

        public void Initialize()
        {
            _speechSynth = new SpeechSynthesizer();

            _speechSynth.SelectVoice(_voice);
            _speechSynth.SetOutputToDefaultAudioDevice();
        }

        public void Communicate(string message)
        {
            _speechSynth.SpeakAsync(message);
        }

        public void StopComminicating()
        {
            _speechSynth.SpeakAsyncCancelAll();
        }
    }
}
