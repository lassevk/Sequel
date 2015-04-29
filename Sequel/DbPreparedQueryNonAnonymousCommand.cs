using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using JetBrains.Annotations;

namespace Sequel
{
    internal class DbPreparedQueryNonAnonymousCommand<T> : DbPreparedQueryCommand<T>
        where T : class, new()
    {
        [CanBeNull, ItemNotNull]
        private List<Tuple<int, PropertyInfo>> _IndexedProperties;

        public DbPreparedQueryNonAnonymousCommand([NotNull] IDbConnection connection, [CanBeNull] IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters)
            : base(connection, transaction, sql, parameters)
        {
        }

        protected override T CreateItem(IDataReader reader)
        {
            if (_IndexedProperties == null)
                _IndexedProperties = CreateIndexedProperties(reader);

            if (_IndexedProperties.Count == 0)
                throw new InvalidOperationException("No properties on the object type matches columns in the result");

            var item = new T();
            foreach (var indexedProperty in _IndexedProperties)
            {
                var value = reader.GetValue(indexedProperty.Item1);
                if (value is DBNull)
                    value = null;

                indexedProperty.Item2.SetValue(item, value, null);
            }
            return item;
        }

        [NotNull, ItemNotNull]
        private List<Tuple<int, PropertyInfo>> CreateIndexedProperties(IDataReader reader)
        {
            var result = new List<Tuple<int, PropertyInfo>>();

            var itemType = typeof(T);

            for (int index = 0; index < reader.FieldCount; index++)
            {
                var name = reader.GetName(index);
                var property = itemType.GetProperty(name);
                if (property == null)
                    continue;

                if (!property.CanWrite)
                    continue;

                result.Add(Tuple.Create(index, property));
            }

            return result;
        }
    }
}