using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;

namespace Sequel
{
    internal abstract class DbPreparedQueryCommand<T> : DbPreparedCommand, IDbPreparedCommand<List<T>>
    {
        protected DbPreparedQueryCommand([NotNull] IDbConnection connection, [CanBeNull] IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters)
            : base(connection, transaction, sql, parameters)
        {
        }

        [NotNull, ItemNotNull]
        public List<T> Execute(object parameterValues = null)
        {
            var result = new List<T>();

            AssignParameters(parameterValues);
            using (var reader = Command.ExecuteReader())
            {
                while (reader.Read())
                    result.Add(CreateItem(reader));
            }

            return result;
        }

        protected abstract T CreateItem(IDataReader reader);
    }
}