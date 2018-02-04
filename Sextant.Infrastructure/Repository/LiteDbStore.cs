// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Sextant.Domain;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sextant.Infrastructure.Repository
{
    public class LiteDbStore<T> : IDataStore<T>
    {
        private readonly LiteDatabase _db;

        private LiteCollection<T> Collection => _db.GetCollection<T>();

        public LiteDbStore(string fileName)
        {
            _db = new LiteDatabase(fileName);
        }

        public bool Drop()                                              => _db.DropCollection(Collection.Name);
        public int Count()                                              => Collection.Count();
        public void Dispose()                                           => _db.Dispose();
        public bool Update(T document)                                  => Collection.Update(document);
        public IEnumerable<T> FindAll()                                 => Collection.FindAll();
        public int Insert(T document)                                   => Collection.Insert(document);
        public int InsertBulk(IEnumerable<T> documents)                 => Collection.InsertBulk(documents);
        public T FindOne(Expression<Func<T, bool>> predicate)           => Collection.FindOne(predicate);
        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate) => Collection.Find(predicate);
    }
}
