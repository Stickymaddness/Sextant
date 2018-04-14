// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using WindowsInput;
using WindowsInput.Native;

namespace Sextant.Infrastructure.VirtualKeyboard
{
    internal static class KeystrokeService
    {
        private static InputSimulator _inputSimulator = new InputSimulator();

        internal static void SendKeystrokes(string keys)
        {
            _inputSimulator.Keyboard.TextEntry(keys);
        }

        internal static void SendEnter()
        {
            _inputSimulator.Keyboard.KeyDown(VirtualKeyCode.RETURN);
            _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RETURN);
        }
    }
}
