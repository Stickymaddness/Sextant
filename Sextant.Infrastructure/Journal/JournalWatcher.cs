/* Copyright 2017 cmdrmcdonald
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *
 * Modifications Copyright 2018 Stickymaddness
 *
 * The original LogMonitor.cs has been copied to JournalWatcher.cs and modified
 */

using Sextant.Domain;
using Sextant.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Sextant.Infrastructure.Journal
{
    public class JournalWatcher : IJournalWatcher
    {
        private static bool running;
        private static string filter = @"^Journal\.[0-9\.]+\.log$";

        private static string _directory;
        private static ILogger _logger;
        private static IJournalHandler _journalHandler;

        public JournalWatcher(ILogger logger, IJournalHandler journalHandler, JournalWatcherSettings settings)
        {
            _logger         = logger;
            _journalHandler = journalHandler;
            _directory      = settings.JournalDirectory;
        }

        public void Initialize() => Task.Factory.StartNew(() => Watch(), TaskCreationOptions.LongRunning);

        private static void Callback(string firstLine)
        {
            try
            {
                _journalHandler.Handle(new List<string> { firstLine });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Exception handling line: {firstLine}");
            }
        }

        private static void Watch()
        {
            try
            {
                start();
            }
            catch (Exception ex) 
            {
                _logger.Error(ex, $"Exception while watching");
            }
        }

        private static void start()
        {
            var Filter = new Regex(filter);

            if (_directory == null || _directory.Trim() == "")
            {
                return;
            }

            running = true;

            // Start off by moving to the end of the file
            long lastSize = 0;
            string lastName = null;
            FileInfo fileInfo = null;
            try
            {
                fileInfo = FindLatestFile(_directory, Filter);
            }
            catch (NotSupportedException)
            {
            }
            if (fileInfo != null)
            {
                lastSize = fileInfo.Length;
                lastName = fileInfo.Name;

                // Elite-specific: start off by grabbing the first line so that we know if we're in beta or live
                using (var fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(fs, Encoding.UTF8))
                {
                    string firstLine = reader.ReadLine() ?? "";
                    // First line should be a file header
                    if (firstLine.Contains("Fileheader"))
                    {
                        // Pass this along as an event
                        Callback(firstLine);
                    }
                }
            }

            // Main loop
            while (running)
            {
                fileInfo = FindLatestFile(_directory, Filter);
                if (fileInfo == null || fileInfo.Name != lastName)
                {
                    lastName = fileInfo == null ? null : fileInfo.Name;
                    lastSize = 0;
                }
                else
                {
                    long thisSize = fileInfo.Length;
                    long seekPos = 0;
                    int readLen = 0;
                    if (lastSize != thisSize)
                    {
                        if (thisSize > lastSize)
                        {
                            // File has been appended - read the remaining info
                            seekPos = lastSize;
                            readLen = (int)(thisSize - lastSize);
                        }
                        else if (thisSize < lastSize)
                        {
                            // File has been truncated - read all of the info
                            seekPos = 0;
                            readLen = (int)thisSize;
                        }

                        using (FileStream fs = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            fs.Seek(seekPos, SeekOrigin.Begin);
                            byte[] bytes = new byte[readLen];
                            int haveRead = 0;
                            while (haveRead < readLen)
                            {
                                haveRead += fs.Read(bytes, haveRead, readLen - haveRead);
                                fs.Seek(seekPos + haveRead, SeekOrigin.Begin);
                            }
                            // Convert bytes to string
                            string s = Encoding.UTF8.GetString(bytes);
                            string[] lines = Regex.Split(s, "\r?\n");
                            foreach (string line in lines)
                            {
                                Callback(line);
                            }
                        }
                    }
                    lastSize = thisSize;
                }
                Thread.Sleep(100);
            }
        }

        public void stop()
        {
            running = false;
        }

        /// <summary>Find the latest file in a given directory matching a given expression, or null if no such file exists</summary>
        private static FileInfo FindLatestFile(string path, Regex filter = null)
        {
            if (path == null)
            {
                // Configuration can be changed underneath us so we do have to check each time...
                return null;
            }

            var directory = new DirectoryInfo(path);
            if (directory != null)
            {
                try
                {
                    FileInfo info = directory.GetFiles().Where(f => filter == null || filter.IsMatch(f.Name)).OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
                    if (info != null)
                    {
                        // This info can be cached so force a refresh
                        info.Refresh();
                    }
                    return info;
                }
                catch (Exception ex)
                {
                    LogOnce(ex);
                }
            }
            return null;
        }

        private static bool lastFileExceptionLogged = false;
        private static void LogOnce(Exception ex)
        {
            if (lastFileExceptionLogged)
                return;

            _logger.Error(ex, $"Exception finding latest journal file in {_directory}");
            lastFileExceptionLogged = true;
        }
    }
}
