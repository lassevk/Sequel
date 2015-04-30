using System;
using System.Data;
using JetBrains.Annotations;

namespace Sequel
{
    internal class DbPreparedQueryScalar<T> : DbPreparedCommand, IDbPreparedCommand<T>
    {
        public DbPreparedQueryScalar([NotNull] IDbConnection connection, [CanBeNull] IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters)
            : base(connection, transaction, sql, parameters)
        {
        }

        public T Execute(object parameterValues = null)
        {
            AssignParameters(parameterValues);
            var value = Command.ExecuteScalar();
            if (value is DBNull)
                value = null;
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}