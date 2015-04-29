using System.Data;
using JetBrains.Annotations;

namespace Sequel
{
    internal class DbPreparedQueryStringScalar : DbPreparedQueryScalar<string>
    {
        public DbPreparedQueryStringScalar([NotNull] IDbConnection connection, [CanBeNull] IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters)
            : base(connection, transaction, sql, parameters)
        {
        }
    }
}