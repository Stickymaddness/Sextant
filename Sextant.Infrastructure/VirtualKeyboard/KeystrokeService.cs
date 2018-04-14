// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Runtime.InteropServices;
using WindowsInput;
using WindowsInput.Native;

namespace Sextant.Infrastructure.VirtualKeyboard
{
    internal static class KeystrokeService
    {
        private static InputSimulator _inputSimulator = new InputSimulator();

        [DllImport("user32.dll")] static extern short VkKeyScan(char ch);

        internal static void SendKeystrokes(string keys)
        {
            _inputSimulator.Keyboard.TextEntry(keys);
            _inputSimulator.Keyboard.Sleep(10);
        }

        internal static void SendSingleKeyPress(string key)
        {
            byte scanCode = new Helper { Value = VkKeyScan(key[0]) }.Low;
            var keyCode   = (VirtualKeyCode)scanCode;

            _inputSimulator.Keyboard.KeyDown(keyCode);
            _inputSimulator.Keyboard.Sleep(10);
            _inputSimulator.Keyboard.KeyUp(keyCode);
        }

        internal static void SendEnter()
        {
            _inputSimulator.Keyboard.KeyDown(VirtualKeyCode.RETURN);
            _inputSimulator.Keyboard.Sleep(10);
            _inputSimulator.Keyboard.KeyUp(VirtualKeyCode.RETURN);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct Helper
    {
        [FieldOffset(0)] public short Value;
        [FieldOffset(0)] public byte Low;
        [FieldOffset(1)] public byte High;
    }
}
