using System.Data;
using JetBrains.Annotations;

namespace Sequel
{
    internal class DbPreparedExecuteCommand : DbPreparedCommand, IDbPreparedCommand<int>
    {
        public DbPreparedExecuteCommand([NotNull] IDbConnection connection, [CanBeNull] IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters)
            : base(connection, transaction, sql, parameters)
        {
        }

        public int Execute(object parameterValues = null)
        {
            AssignParameters(parameterValues);
            return Command.ExecuteNonQuery();
        }
    }
}