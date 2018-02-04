// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace Sextant.Infrastructure.Journal
{
    internal static class JournalReader
    {
        private static Dictionary<string, long> _fileDictionary = new Dictionary<string, long>();

        internal static void Initialize(string journalPath, string pattern)
        {
            string[] journalFilePaths = Directory.GetFiles(journalPath, pattern);

            foreach (var filePath in journalFilePaths)
            {
                long fileSize             = new FileInfo(filePath).Length;
                _fileDictionary[filePath] = fileSize;
            }
        }

        internal static List<string> ReadJournal(string filePath)
        {
            if (!_fileDictionary.ContainsKey(filePath))
                _fileDictionary[filePath] = 0;

            long offset = _fileDictionary[filePath];

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fs))
            {
                reader.BaseStream.Seek(offset, SeekOrigin.Begin);

                var lines = new List<string>();
                var line  = string.Empty;

                while ((line = reader.ReadLine()) != null)
                    lines.Add(line);

                long newSize = new FileInfo(filePath).Length;
                _fileDictionary[filePath] = newSize;

                return lines;
            }
        }
    }
}
