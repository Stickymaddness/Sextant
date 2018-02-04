// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using Sextant.Infrastructure.Repository;
using Sextant.Tests.Builders;

namespace Sextant.Tests.Commands
{
    public class CommandTestBase
    {
        protected T CreatePhrases<T>() where T : class, new()                         => TestPhraseBuilder.Build<T>();
        protected TestCommunicator CreateCommunicator()                               => new TestCommunicator();
        protected PlayerStatusRepository CreatePlayerStatusRepository()               => new PlayerStatusRepository(new MemoryDataStore<PlayerStatus>());
        protected IDataStore<StarSystemDocument> CreateDataStore()                    => new MemoryDataStore<StarSystemDocument>();
        protected Navigator CreateNavigator()                                         => new Navigator(new NavigationRepository(CreateDataStore()));
        protected Navigator CreateNavigator(IDataStore<StarSystemDocument> dataStore) => new Navigator(new NavigationRepository(dataStore));
    }
}
