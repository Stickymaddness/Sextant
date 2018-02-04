// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using Sextant.Domain;
using Sextant.Domain.Commands;
using Sextant.Domain.Phrases;
using Sextant.Infrastructure;
using Sextant.Infrastructure.Journal;
using Sextant.Infrastructure.Repository;
using Sextant.Infrastructure.Settings;
using Sextant.Infrastructure.TextToSpeech;
using SimpleInjector;
using System.Linq;

namespace Sextant.Host
{
    internal class Bootstrapper
    {
        private const string ExpeditionFileName   = "expedition.db";
        private const string PlayerStatusFileName = "player_status.db";

        internal void Bootstrap(string basePath, Container container)
        {
            InitializeSettings(basePath, container);

            container.Register<ILogger, SerilogWrapper>(Lifestyle.Singleton);

            RegisterDataStore<StarSystemDocument>(container, ExpeditionFileName);
            RegisterDataStore<PlayerStatus>(container, PlayerStatusFileName);

            container.Register<INavigator, Navigator>(Lifestyle.Singleton);
            container.Register<ICommunicator, VoiceCommunicator>(Lifestyle.Singleton);

            container.Register<IUserDataService, ClipboardDataService>(Lifestyle.Singleton);
            container.Register<INavigationRepository, NavigationRepository>(Lifestyle.Singleton);
            container.Register<IPlayerStatus, PlayerStatusRepository>(Lifestyle.Singleton);

            RegisterCommands(container);

            container.Register<ICommandExecutor, CommandExecutor>(Lifestyle.Singleton);
            container.Register<ICommandRegistry, CommandRegistry>(Lifestyle.Singleton);

            container.Register<IJournalHandler, JournalHandler>();
            container.Register<IJournalWatcher, JournalWatcher>();

            container.Register<IGalaxyMap, GalaxyMapInteractor>();

            container.Verify();
        }

        private void InitializeSettings(string basePath, Container container)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(basePath)
                                                                         .AddJsonFile("settings.json")
                                                                         .AddJsonFile("phrases.json")
                                                                         .Build();

            RegisterPhrases(container, configuration);

            container.Register(() => configuration.LoadSettings<JournalWatcherSettings>("JournalWatcher"));
            container.Register(() => configuration.LoadSettings<GalaxyMapInteractorSettings>("GalaxyMapInteractor"));
            container.Register(() => configuration.LoadSettings<VoiceCommunicatorSettings>("VoiceCommunicator"), Lifestyle.Singleton);
            container.Register(() => configuration.LoadSettings<Preferences>("Preferences"), Lifestyle.Singleton);
        }

        private static void RegisterPhrases(Container container, IConfigurationRoot configuration)
        {
            RegisterPhrase<JumpPhrases>("Jump", container, configuration);
            RegisterPhrase<DockSRVPhrases>("DockSRV", container, configuration);
            RegisterPhrase<GameLoadPhrases>("GameLoad", container, configuration);
            RegisterPhrase<NextScanPhrases>("NextScan", container, configuration);
            RegisterPhrase<ProgressPhrases>("Progress", container, configuration);
            RegisterPhrase<GreetingPhrases>("Greeting", container, configuration);
            RegisterPhrase<LaunchSRVPhrases>("LaunchSRV", container, configuration);
            RegisterPhrase<SkipSystemPhrases>("SkipSystem", container, configuration);
            RegisterPhrase<RescanSystemPhrases>("RescanSystem", container, configuration);
            RegisterPhrase<InitializedPhrases>("Initialized", container, configuration);
            RegisterPhrase<CelestialScanPhrases>("CelestialScan", container, configuration);
            RegisterPhrase<GetNextSystemPhrases>("GetNextSystem", container, configuration);
            RegisterPhrase<FindNextSystemPhrases>("FindNextSystem", container, configuration);
            RegisterPhrase<ScansRemainingPhrases>("ScansRemaining", container, configuration);
            RegisterPhrase<PlotExpeditionPhrases>("PlotExpedition", container, configuration);
            RegisterPhrase<CancelExpeditionPhrases>("CancelExpedition", container, configuration);
        }

        private static void RegisterPhrase<T>(string key, Container container, IConfigurationRoot configuration)
            where T : class, new() => container.Register(() => configuration.LoadSettings<T>(key));

        private static void RegisterDataStore<T>(Container container, string fileName) =>
            container.Register<IDataStore<T>>(() => new LiteDbStore<T>(fileName), Lifestyle.Singleton);

        private static void RegisterCommands(Container container)
        {
            var commandAssembly = typeof(ICommand).Assembly;

            var registrations = commandAssembly.GetExportedTypes()
                                               .Where(t => t.Name != "CommandRegistry")
                                               .Where(t => t.Name != "CommandExecutor")
                                               .Where(t => t.Namespace == "Sextant.Domain.Commands")
                                               .Where(t => t.IsAbstract == false)
                                               .Where(t => t.GetInterfaces().Any());

            container.RegisterCollection<ICommand>(registrations);
        }
    }
}
