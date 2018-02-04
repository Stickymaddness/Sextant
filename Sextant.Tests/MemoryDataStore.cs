// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sextant.Tests
{
    public class MemoryDataStore<T> : IDataStore<T>
    {
        private Dictionary<int, T> _db = new Dictionary<int, T>();

        public int Count()                                              => _db.Count();
        public IEnumerable<T> FindAll()                                 => _db.Values;
        public T FindOne(Expression<Func<T, bool>> predicate)           => _db.Values.FirstOrDefault(predicate.Compile());
        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate) => _db.Values.Where(predicate.Compile());

        public bool Drop()
        {
            _db.Clear();
            return true;
        }

        public int Insert(T document)
        {
            _db.Add(document.GetHashCode(), document);
            return 1;
        }

        public int InsertBulk(IEnumerable<T> documents)
        {
            foreach (var document in documents)
                Update(document);

            return documents.Count();
        }

        public bool Update(T document)
        {
            _db[document.GetHashCode()] = document;
            return true;
        }

        public void Dispose()
        {
            _db.Clear();
            _db = null;
        }
    }
}
