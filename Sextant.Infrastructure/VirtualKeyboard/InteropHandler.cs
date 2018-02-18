//Modified version of https://github.com/nomadyow/Ocellus/blob/master/OcellusPlugin/KeyMouse.cs

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Sextant.Infrastructure.VirtualKeyboard
{    
    internal static class InteropHandler
    {
        internal static uint ConvertKey(char c)
        {
            return MapVkToScanCodeEx(new VK_Helper { Value = VkKeyScan(c) }.Low);
        }

        internal static void KeyPress(IReadOnlyCollection<uint> scanCodesEx, int duration = 100)
        {
            KeyDown(scanCodesEx);
            Thread.Sleep(duration);
            KeyUp(scanCodesEx);
        }

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")] static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        [StructLayout(LayoutKind.Explicit)]
        struct VK_Helper
        {
            [FieldOffset(0)] public short Value;
            [FieldOffset(0)] public byte Low;
            [FieldOffset(1)] public byte High;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            private readonly int dx;
            private readonly int dy;
            private readonly uint mouseData;
            private readonly uint dwFlags;
            private readonly uint time;
            private readonly IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public readonly uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public readonly uint uMsg;
            public readonly ushort wParamL;
            public readonly ushort wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT_UNION
        {
            [FieldOffset(0)]
            public readonly MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public readonly HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public INPUT_UNION u;
        }

        private const int INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_SCANCODE = 0x0008;
        private const uint MAPVK_VK_TO_VSC = 0;

        private static void KeyDown(IReadOnlyCollection<uint> scanCodesEx)
        {
            SendInput((uint)scanCodesEx.Count, BuildInputs(scanCodesEx, true), Marshal.SizeOf(typeof(INPUT)));
        }

        private static void KeyUp(IReadOnlyCollection<uint> scanCodesEx)
        {
            SendInput((uint)scanCodesEx.Count, BuildInputs(scanCodesEx, false), Marshal.SizeOf(typeof(INPUT)));
        }

        private static INPUT[] BuildInputs(IReadOnlyCollection<uint> scanCodesEx, bool keyDown)
        {
            var size = scanCodesEx.Count;
            var i = 0;
            var inputs = new INPUT[size];
            foreach (var scanCodeEx in scanCodesEx)
            {
                // reverse order for keyUp
                inputs[keyDown ? i : size - i - 1] = BuildInput(scanCodeEx, keyDown);
                i++;
            }
            return inputs;
        }

        private static INPUT BuildInput(uint scanCodeEx, bool keyDown)
        {
            var keyDownOrUp = keyDown ? 0 : KEYEVENTF_KEYUP;
            var keyIsExtended = (scanCodeEx & 0x80000000) == 0 ? 0 : KEYEVENTF_EXTENDEDKEY;
            var keybdInput = new KEYBDINPUT
            {
                wVk = 0,
                wScan = (ushort)(scanCodeEx & 0x000000FF),
                dwFlags = KEYEVENTF_SCANCODE | keyDownOrUp | keyIsExtended,
                dwExtraInfo = GetMessageExtraInfo()
            };
            var inputUnion = new INPUT_UNION { ki = keybdInput };
            return new INPUT { type = INPUT_KEYBOARD, u = inputUnion };
        }

        private static uint MapVkToScanCodeEx(uint vkCode)
        {
            var extended = VK_EXTENDED.Contains((VK)vkCode);
            if ((vkCode & 0xe1000000) > 0)
            {
                extended = true;
                vkCode &= 0xff;
            }
            var scanCode = MapVirtualKey(vkCode, MAPVK_VK_TO_VSC);
            return extended ? scanCode | 0x80000000 : scanCode;
        }

        private static readonly List<VK> VK_EXTENDED = new List<VK>
        {
            VK.VK_CANCEL,
            VK.VK_PRIOR,
            VK.VK_NEXT,
            VK.VK_END,
            VK.VK_HOME,
            VK.VK_LEFT,
            VK.VK_UP,
            VK.VK_RIGHT,
            VK.VK_DOWN,
            VK.VK_SNAPSHOT,
            VK.VK_INSERT,
            VK.VK_DELETE,
            VK.VK_LWIN,
            VK.VK_RWIN,
            VK.VK_APPS,
            VK.VK_DIVIDE,
            VK.VK_NUMLOCK,
            VK.VK_RCONTROL,
            VK.VK_RMENU
        };

        private enum VK : uint
        {
            // Virtual Keys, Standard Set
            VK_LBUTTON = 0x01,
            VK_RBUTTON = 0x02,
            VK_CANCEL = 0x03,
            VK_MBUTTON = 0x04, // NOT contiguous with L & RBUTTON
            VK_XBUTTON1 = 0x05, // NOT contiguous with L & RBUTTON
            VK_XBUTTON2 = 0x06, // NOT contiguous with L & RBUTTON

            // 0x07 : unassigned

            VK_BACK = 0x08,
            VK_TAB = 0x09,

            // 0x0A - 0x0B : reserved

            VK_CLEAR = 0x0C,
            VK_RETURN = 0x0D,

            // 0x0E - 0x0F : unassigned

            VK_SHIFT = 0x10,
            VK_CONTROL = 0x11,
            VK_MENU = 0x12,
            VK_PAUSE = 0x13,
            VK_CAPITAL = 0x14,

            VK_KANA = 0x15,
            VK_HANGEUL = 0x15, // old name - should be here for compatibility
            VK_HANGUL = 0x15,

            // 0x16 : unassigned

            VK_JUNJA = 0x17,
            VK_FINAL = 0x18,
            VK_HANJA = 0x19,
            VK_KANJI = 0x19,

            // 0x1A : unassigned

            VK_ESCAPE = 0x1B,

            VK_CONVERT = 0x1C,
            VK_NONCONVERT = 0x1D,
            VK_ACCEPT = 0x1E,
            VK_MODECHANGE = 0x1F,

            VK_SPACE = 0x20,
            VK_PRIOR = 0x21,
            VK_NEXT = 0x22,
            VK_END = 0x23,
            VK_HOME = 0x24,
            VK_LEFT = 0x25,
            VK_UP = 0x26,
            VK_RIGHT = 0x27,
            VK_DOWN = 0x28,
            VK_SELECT = 0x29,
            VK_PRINT = 0x2A,
            VK_EXECUTE = 0x2B,
            VK_SNAPSHOT = 0x2C,
            VK_INSERT = 0x2D,
            VK_DELETE = 0x2E,
            VK_HELP = 0x2F,

            // VK_0 - VK_9 are the same as ASCII '0' - '9'
            VK_0 = 0x30,
            VK_1 = 0x31,
            VK_2 = 0x32,
            VK_3 = 0x33,
            VK_4 = 0x34,
            VK_5 = 0x35,
            VK_6 = 0x36,
            VK_7 = 0x37,
            VK_8 = 0x38,
            VK_9 = 0x39,

            // 0x3A - 0x40 : unassigned

            // VK_A - VK_Z are the same as ASCII 'A' - 'Z'
            VK_A = 0x41,
            VK_B = 0x42,
            VK_C = 0x43,
            VK_D = 0x44,
            VK_E = 0x45,
            VK_F = 0x46,
            VK_G = 0x47,
            VK_H = 0x48,
            VK_I = 0x49,
            VK_J = 0x4A,
            VK_K = 0x4B,
            VK_L = 0x4C,
            VK_M = 0x4D,
            VK_N = 0x4E,
            VK_O = 0x4F,
            VK_P = 0x50,
            VK_Q = 0x51,
            VK_R = 0x52,
            VK_S = 0x53,
            VK_T = 0x54,
            VK_U = 0x55,
            VK_V = 0x56,
            VK_W = 0x57,
            VK_X = 0x58,
            VK_Y = 0x59,
            VK_Z = 0x5A,

            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C,
            VK_APPS = 0x5D,

            // 0x5E : reserved

            VK_SLEEP = 0x5F,

            VK_NUMPAD0 = 0x60,
            VK_NUMPAD1 = 0x61,
            VK_NUMPAD2 = 0x62,
            VK_NUMPAD3 = 0x63,
            VK_NUMPAD4 = 0x64,
            VK_NUMPAD5 = 0x65,
            VK_NUMPAD6 = 0x66,
            VK_NUMPAD7 = 0x67,
            VK_NUMPAD8 = 0x68,
            VK_NUMPAD9 = 0x69,
            VK_MULTIPLY = 0x6A,
            VK_ADD = 0x6B,
            VK_SEPARATOR = 0x6C,
            VK_SUBTRACT = 0x6D,
            VK_DECIMAL = 0x6E,
            VK_DIVIDE = 0x6F,
            VK_F1 = 0x70,
            VK_F2 = 0x71,
            VK_F3 = 0x72,
            VK_F4 = 0x73,
            VK_F5 = 0x74,
            VK_F6 = 0x75,
            VK_F7 = 0x76,
            VK_F8 = 0x77,
            VK_F9 = 0x78,
            VK_F10 = 0x79,
            VK_F11 = 0x7A,
            VK_F12 = 0x7B,
            VK_F13 = 0x7C,
            VK_F14 = 0x7D,
            VK_F15 = 0x7E,
            VK_F16 = 0x7F,
            VK_F17 = 0x80,
            VK_F18 = 0x81,
            VK_F19 = 0x82,
            VK_F20 = 0x83,
            VK_F21 = 0x84,
            VK_F22 = 0x85,
            VK_F23 = 0x86,
            VK_F24 = 0x87,

            // 0x88 - 0x8F : unassigned

            VK_NUMLOCK = 0x90,
            VK_SCROLL = 0x91,

            // NEC PC-9800 kbd definitions
            VK_OEM_NEC_EQUAL = 0x92, // '=' key on numpad

            // Fujitsu/OASYS kbd definitions
            VK_OEM_FJ_JISHO = 0x92, // 'Dictionary' key
            VK_OEM_FJ_MASSHOU = 0x93, // 'Unregister word' key
            VK_OEM_FJ_TOUROKU = 0x94, // 'Register word' key
            VK_OEM_FJ_LOYA = 0x95, // 'Left OYAYUBI' key
            VK_OEM_FJ_ROYA = 0x96, // 'Right OYAYUBI' key

            // 0x97 - 0x9F : unassigned

            // VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys.
            // Used only as parameters to GetAsyncKeyState() and GetKeyState().
            // No other API or message will distinguish left and right keys in this way.
            VK_LSHIFT = 0xA0,
            VK_RSHIFT = 0xA1,
            VK_LCONTROL = 0xA2,
            VK_RCONTROL = 0xA3,
            VK_LMENU = 0xA4,
            VK_RMENU = 0xA5,

            VK_BROWSER_BACK = 0xA6,
            VK_BROWSER_FORWARD = 0xA7,
            VK_BROWSER_REFRESH = 0xA8,
            VK_BROWSER_STOP = 0xA9,
            VK_BROWSER_SEARCH = 0xAA,
            VK_BROWSER_FAVORITES = 0xAB,
            VK_BROWSER_HOME = 0xAC,

            VK_VOLUME_MUTE = 0xAD,
            VK_VOLUME_DOWN = 0xAE,
            VK_VOLUME_UP = 0xAF,
            VK_MEDIA_NEXT_TRACK = 0xB0,
            VK_MEDIA_PREV_TRACK = 0xB1,
            VK_MEDIA_STOP = 0xB2,
            VK_MEDIA_PLAY_PAUSE = 0xB3,
            VK_LAUNCH_MAIL = 0xB4,
            VK_LAUNCH_MEDIA_SELECT = 0xB5,
            VK_LAUNCH_APP1 = 0xB6,
            VK_LAUNCH_APP2 = 0xB7,

            // 0xB8 - 0xB9 : reserved

            VK_OEM_1 = 0xBA, // ';:' for US
            VK_OEM_PLUS = 0xBB, // '+' any country
            VK_OEM_COMMA = 0xBC, // ',' any country
            VK_OEM_MINUS = 0xBD, // '-' any country
            VK_OEM_PERIOD = 0xBE, // '.' any country
            VK_OEM_2 = 0xBF, // '/?' for US
            VK_OEM_3 = 0xC0, // '`~' for US

            // 0xC1 - 0xD7 : reserved
            // 0xD8 - 0xDA : unassigned

            VK_OEM_4 = 0xDB, //  '[{' for US
            VK_OEM_5 = 0xDC, //  '\|' for US
            VK_OEM_6 = 0xDD, //  ']}' for US
            VK_OEM_7 = 0xDE, //  ''"' for US
            VK_OEM_8 = 0xDF,

            // 0xE0 : reserved

            // Various extended or enhanced keyboards
            VK_OEM_AX = 0xE1, //  'AX' key on Japanese AX kbd
            VK_OEM_102 = 0xE2, //  "<>" or "\|" on RT 102-key kbd.
            VK_ICO_HELP = 0xE3, //  Help key on ICO
            VK_ICO_00 = 0xE4, //  00 key on ICO

            VK_PROCESSKEY = 0xE5,

            VK_ICO_CLEAR = 0xE6,

            VK_PACKET = 0xE7,

            // 0xE8 : unassigned

            // Nokia/Ericsson definitions
            VK_OEM_RESET = 0xE9,
            VK_OEM_JUMP = 0xEA,
            VK_OEM_PA1 = 0xEB,
            VK_OEM_PA2 = 0xEC,
            VK_OEM_PA3 = 0xED,
            VK_OEM_WSCTRL = 0xEE,
            VK_OEM_CUSEL = 0xEF,
            VK_OEM_ATTN = 0xF0,
            VK_OEM_FINISH = 0xF1,
            VK_OEM_COPY = 0xF2,
            VK_OEM_AUTO = 0xF3,
            VK_OEM_ENLW = 0xF4,
            VK_OEM_BACKTAB = 0xF5,

            VK_ATTN = 0xF6,
            VK_CRSEL = 0xF7,
            VK_EXSEL = 0xF8,
            VK_EREOF = 0xF9,
            VK_PLAY = 0xFA,
            VK_ZOOM = 0xFB,
            VK_NONAME = 0xFC,
            VK_PA1 = 0xFD,
            VK_OEM_CLEAR = 0xFE

            // 0xFF : reserved
        }
    }
}
