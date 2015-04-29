using System.Data;
using JetBrains.Annotations;

namespace Sequel
{
    internal class DbPreparedQueryNullableScalar<T> : DbPreparedQueryScalar<T?>
        where T : struct
    {
        public DbPreparedQueryNullableScalar([NotNull] IDbConnection connection, [CanBeNull] IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters)
            : base(connection, transaction, sql, parameters)
        {
        }
    }
}