using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;

namespace Sequel
{
    public static class DbTransactionExtensions
    {
        [PublicAPI]
        public static int Execute([NotNull] this IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            using (var command = transaction.PrepareForExecute(sql, parameters))
            {
                return command.Execute();
            }
        }

        [PublicAPI, NotNull]
        public static IDbPreparedCommand<int> PrepareForExecute([NotNull] this IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (sql == null)
                throw new ArgumentNullException("sql");

            return new DbPreparedExecuteCommand(transaction.Connection, transaction, sql, parameters);
        }

        [PublicAPI, NotNull, ItemCanBeNull]
        public static List<T> Query<T>([NotNull] this IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters = null)
            where T : class, new()
        {
            using (var command = transaction.PrepareForQuery<T>(sql, parameters))
            {
                return command.Execute();
            }
        }

        [PublicAPI, NotNull]
        public static IDbPreparedCommand<List<T>> PrepareForQuery<T>([NotNull] this IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters = null)
            where T : class, new()
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (sql == null)
                throw new ArgumentNullException("sql");
            return new DbPreparedQueryNonAnonymousCommand<T>(transaction.Connection, transaction, sql, parameters);
        }

        [PublicAPI, NotNull, ItemCanBeNull]
        public static List<T> QueryAnonymous<T>(this IDbTransaction transaction, string sql, [CanBeNull] T template = null, object parameters = null)
            where T : class
        {
            using (var command = transaction.PrepareForQueryAnonymous<T>(sql, template, parameters))
            {
                return command.Execute();
            }
        }

        [PublicAPI, NotNull]
        public static IDbPreparedCommand<List<T>> PrepareForQueryAnonymous<T>([NotNull] this IDbTransaction transaction, [NotNull] string sql, [CanBeNull] T template = null, [CanBeNull] object parameters = null)
            where T : class
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (sql == null)
                throw new ArgumentNullException("sql");

            return new DbPreparedQueryAnonymousCommand<T>(transaction.Connection, transaction, sql, parameters);
        }

        [PublicAPI, NotNull, ItemCanBeNull]
        public static List<T> QuerySequence<T>([NotNull] this IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            using (var command = transaction.PrepareForQuerySequence<T>(sql, parameters))
            {
                return command.Execute();
            }
        }

        [PublicAPI, NotNull]
        public static IDbPreparedCommand<List<T>> PrepareForQuerySequence<T>([NotNull] this IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (sql == null)
                throw new ArgumentNullException("sql");

            return new DbPreparedQuerySequenceCommand<T>(transaction.Connection, transaction, sql, parameters);
        }

        [PublicAPI, CanBeNull]
        public static T QueryScalar<T>([NotNull] this IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            using (var command = transaction.PrepareForQueryScalar<T>(sql, parameters))
            {
                return command.Execute();
            }
        }

        [PublicAPI, NotNull]
        public static IDbPreparedCommand<T> PrepareForQueryScalar<T>([NotNull] this IDbTransaction transaction, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (sql == null)
                throw new ArgumentNullException("sql");

            return new DbPreparedQueryScalar<T>(transaction.Connection, transaction, sql, parameters);
        }
    }
}