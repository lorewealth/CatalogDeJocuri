using AboutGame;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stocking.Database
{
    internal class TableResolver<T> where T : class, IIdentifiable
    {
        private readonly Dictionary<string, T> cache;
        private readonly SqliteConnection db;
        private readonly string tableName;
        private readonly Func<int, string, T> createEntity;

        private TableResolver(SqliteConnection db, Dictionary<string, T> cache, string tableName, Func<int, string, T> createEntity)
        {
            this.db = db;
            this.cache = cache;
            this.tableName = tableName;
            this.createEntity = createEntity;
        }

        public static async Task<TableResolver<T>> CreateAsync(SqliteConnection db, string tableName, Func<int, string, T> createEntity)
        {
            // SQLite returns INTEGER primary keys as Int64.  Materializing the
            // domain types directly is fragile because their constructors accept Int32.
            var existing = await db.QueryAsync<EntityRow>("SELECT Id, Name FROM " + tableName + ";");
            var cache = existing
                .Select(row => createEntity(checked((int)row.Id), row.Name))
                .ToDictionary(entity => entity.Name, StringComparer.OrdinalIgnoreCase);
            return new TableResolver<T>(db, cache, tableName, createEntity);
        }

        private sealed class EntityRow
        {
            public long Id { get; init; }
            public string Name { get; init; } = "";
        }

        public async Task<T> ResolveAsync(string name, IDbTransaction? transaction = null)
        {
            if (cache.TryGetValue(name, out var entity)) 
                return entity;

            int newId = await db.ExecuteScalarAsync<int>(
                "INSERT INTO " + tableName + " (Name) VALUES (@Name); SELECT last_insert_rowid();",
                new { Name = name }, transaction);

            var newEntity = createEntity(newId, name);
            cache[name] = newEntity;
            return newEntity;
        }
    }
}
