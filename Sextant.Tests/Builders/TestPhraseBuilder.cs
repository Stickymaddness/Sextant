// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;

namespace Sextant.Tests.Builders
{
    public static class TestPhraseBuilder
    {
        public static T Build<T>()
            where T : new()
        {
            T result = new T();

            foreach (PropertyInfo info in result.GetType().GetProperties())
            {
                if (info.CanRead && info.PropertyType.IsArray)
                    info.SetValue(result, new[] { info.Name }, null);

                else if (info.CanRead)
                    info.SetValue(result, info.Name, null);
            }

            return result;
        }
    }
}