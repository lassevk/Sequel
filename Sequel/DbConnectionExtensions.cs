using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;

namespace Sequel
{
    [PublicAPI]
    public static class DbConnectionExtensions
    {
        [PublicAPI, NotNull]
        public static IDbConnection OpenNow([NotNull] this IDbConnection connection)
        {
            connection.Open();
            return connection;
        }

        [PublicAPI]
        public static int Execute([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            using (var command = connection.PrepareForExecute(sql, parameters))
            {
                return command.Execute();
            }
        }

        [PublicAPI, NotNull]
        public static IDbPreparedCommand<int> PrepareForExecute([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (sql == null)
                throw new ArgumentNullException("sql");

            return new DbPreparedExecuteCommand(connection, null, sql, parameters);
        }

        [PublicAPI, NotNull, ItemCanBeNull]
        public static List<T> Query<T>([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] object parameters = null)
            where T : class, new()
        {
            using (var command = connection.PrepareForQuery<T>(sql, parameters))
            {
                return command.Execute();
            }
        }

        [PublicAPI, NotNull]
        public static IDbPreparedCommand<List<T>> PrepareForQuery<T>([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] object parameters = null)
            where T : class, new()
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (sql == null)
                throw new ArgumentNullException("sql");
            return new DbPreparedQueryNonAnonymousCommand<T>(connection, null, sql, parameters);
        }

        [PublicAPI, NotNull, ItemCanBeNull]
        public static List<T> QueryAnonymous<T>(this IDbConnection connection, string sql, [CanBeNull] T template = null, object parameters = null)
            where T : class
        {
            using (var command = connection.PrepareForQueryAnonymous<T>(sql, template, parameters))
            {
                return command.Execute();
            }
        }

        [PublicAPI, NotNull]
        public static IDbPreparedCommand<List<T>> PrepareForQueryAnonymous<T>([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] T template = null, [CanBeNull] object parameters = null)
            where T : class
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (sql == null)
                throw new ArgumentNullException("sql");
            return new DbPreparedQueryAnonymousCommand<T>(connection, null, sql, parameters);
        }

        [PublicAPI, NotNull, ItemCanBeNull]
        public static List<T> QuerySequence<T>([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            using (var command = connection.PrepareForQuerySequence<T>(sql, parameters))
            {
                return command.Execute();
            }
        }

        [PublicAPI, NotNull]
        public static IDbPreparedCommand<List<T>> PrepareForQuerySequence<T>([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (sql == null)
                throw new ArgumentNullException("sql");

            return new DbPreparedQuerySequenceCommand<T>(connection, null, sql, parameters);
        }

        [PublicAPI, CanBeNull]
        public static T? QueryScalar<T>([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] object parameters = null)
            where T : struct
        {
            using (var command = connection.PrepareForQueryScalar<T>(sql, parameters))
            {
                return command.Execute();
            }
        }

        [PublicAPI, NotNull]
        public static IDbPreparedCommand<T?> PrepareForQueryScalar<T>([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] object parameters = null)
            where T : struct
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (sql == null)
                throw new ArgumentNullException("sql");

            return new DbPreparedQueryNullableScalar<T>(connection, null, sql, parameters);
        }

        [PublicAPI, CanBeNull]
        public static string QueryStringScalar([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            using (var command = connection.PrepareForQueryStringScalar(sql, parameters))
            {
                return command.Execute();
            }
        }

        [PublicAPI, NotNull]
        public static IDbPreparedCommand<string> PrepareForQueryStringScalar([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (sql == null)
                throw new ArgumentNullException("sql");

            return new DbPreparedQueryStringScalar(connection, null, sql, parameters);
        }
    }
}