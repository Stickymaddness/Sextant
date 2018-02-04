// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Serilog;
using Sextant.Domain;
using Sextant.Domain.Commands;
using Sextant.Infrastructure;
using Sextant.Infrastructure.Journal;
using SimpleInjector;
using System;

namespace Sextant.Host
{
    public class SextantHost
    {
        private static readonly Container container = new Container();
        private static ICommandExecutor _executor;
        private static Serilog.ILogger _logger;
        private readonly string _pluginName;

        public SextantHost(string basePath, string pluginName)
        {
            _pluginName = pluginName;
            InitializeLogging();

            try
            {
                new Bootstrapper().Bootstrap(basePath, container);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception during bootstrapping");
            }
        }

        public void Initialize()
        {
            _executor        = container.GetInstance<ICommandExecutor>();
            var communicator = container.GetInstance<ICommunicator>();
            var watcher      = container.GetInstance<IJournalWatcher>();

            watcher.Initialize();
            communicator.Initialize();

            _executor.Handle(new VoiceAttackEvent("initialized"));

            _logger.Information($"{_pluginName} Initialized");
        }

        public void Handle(string context)
        {
            try
            {
                _executor.Handle(EventFactory.FromVoiceAttack(context));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Exception handling VoiceAttack command. Context : {context}");
            }
        }

        private void InitializeLogging()
        {
            Log.Logger = new LoggerConfiguration()
                             .Enrich.WithProperty("PluginVersion", _pluginName)
                             .MinimumLevel.Information()
                             .WriteTo.RollingFile("log-{Date}.txt", Serilog.Events.LogEventLevel.Information, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{PluginVersion}][{Level}] {Message}{NewLine}{Exception}")
                             .CreateLogger();

            _logger = Log.Logger;
        }
    }
}
