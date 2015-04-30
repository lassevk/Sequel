using System;
using System.Data;
using JetBrains.Annotations;

namespace Sequel
{
    internal class DbPreparedQuerySequenceCommand<T> : DbPreparedQueryCommand<T>
    {
        public DbPreparedQuerySequenceCommand([NotNull] IDbConnection connection, [CanBeNull] IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters)
            : base(connection, transaction, sql, parameters)
        {
        }

        protected override T CreateItem(IDataReader reader)
        {
            var value = reader.GetValue(0);
            if (value is DBNull)
                value = null;
            return (T) Convert.ChangeType(value, typeof(T));
        }
    }
}