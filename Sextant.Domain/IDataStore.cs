// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sextant.Domain
{
    public interface IDataStore<T> : IDisposable
    {
        bool Drop();
        int Count();
        int Insert(T document);
        int InsertBulk(IEnumerable<T> documents);
        bool Update(T document);
        IEnumerable<T> FindAll();
        T FindOne(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    }
}
