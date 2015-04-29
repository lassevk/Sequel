using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Sequel
{
    internal class DbPreparedQueryAnonymousCommand<T> : DbPreparedQueryCommand<T>
        where T : class
    {
        [CanBeNull, ItemNotNull]
        private List<Tuple<int, int>> _IndexedParameters;

        [CanBeNull]
        private ConstructorInfo _Constructor;

        public DbPreparedQueryAnonymousCommand([NotNull] IDbConnection connection, [CanBeNull] IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters)
            : base(connection, transaction, sql, parameters)
        {
        }

        protected override T CreateItem(IDataReader reader)
        {
            if (_Constructor == null)
                _Constructor = FindConstructor();
            if (_IndexedParameters == null)
                _IndexedParameters = CreateIndexedParameters(reader, _Constructor);

            var parameters = new object[_Constructor.GetParameters().Length];
            foreach (var indexedParameter in _IndexedParameters)
            {
                var value = reader.GetValue(indexedParameter.Item1);
                if (value is DBNull)
                    value = null;
                parameters[indexedParameter.Item2] = value;
            }
            return (T)_Constructor.Invoke(parameters);
        }

        [NotNull]
        private ConstructorInfo FindConstructor()
        {
            return typeof(T).GetConstructors()[0];
        }

        [NotNull, ItemNotNull]
        private List<Tuple<int, int>> CreateIndexedParameters(IDataReader reader, ConstructorInfo constructor)
        {
            var parameters = constructor
                .GetParameters()
                .Select((parameter, index) => new { name = parameter.Name, index })
                .ToDictionary(item => item.name, item => item.index);

            var result = new List<Tuple<int, int>>();
            for (int index = 0; index < reader.FieldCount; index++)
            {
                var name = reader.GetName(index);

                int parameterIndex;
                if (!parameters.TryGetValue(name, out parameterIndex))
                    continue;

                result.Add(Tuple.Create(index, parameterIndex));
            }
            return result;
        }
    }
}