// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace Sextant.Host
{
    internal static class ConfigurationRootExtensions
    {
        public static T LoadSettings<T>(this IConfigurationRoot configuration, string section)
            where T : new()
        {
            T settings = new T();
            configuration.GetSection(section).Bind(settings);

            return settings;
        }
    }
}
