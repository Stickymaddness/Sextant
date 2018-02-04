// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Collections.Generic;

namespace Sextant.Infrastructure.VirtualKeyboard
{
    internal static class KeystrokeService
    {
        private const uint _enterCode = 28;

        internal static void SendKeystokes(string keys)
        {
            var keyList          = keys.ToUpper().ToCharArray();
            List<uint> scanCodes = keyList.Select(k => InteropHandler.ConvertKey(k)).ToList();

            InteropHandler.KeyPress(scanCodes);
        }

        internal static void SendKeystoke(uint code)
        {
            InteropHandler.KeyPress(new List<uint> { code });
        }

        internal static void SendEnter()
        {
            InteropHandler.KeyPress(new List<uint> { _enterCode });
        }
    }
}
