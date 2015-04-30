using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;

namespace Sequel
{
    /// <summary>
    /// This class adds extension methods for <see cref="IDbConnection"/> to execute SQL statements
    /// more easily.
    /// </summary>
    [PublicAPI]
    public static class DbConnectionExtensions
    {
        /// <summary>
        /// Opens the <see cref="IDbConnection"/> and returns it, meant to simplify <code>using (...) { ... }</code>
        /// constructs.
        /// </summary>
        /// <param name="connection">
        /// The <see cref="IDbConnection"/> to open.
        /// </param>
        /// <returns>
        /// The opened <paramref name="connection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="connection"/> is <c>null</c>.</para>
        /// </exception>
        /// <example><code>
        ///     using (var connection = new SQLiteConnection("...").OpenNow())
        ///     using (var transaction = connection.BeginTransaction())
        ///     {
        ///         transaction.Execute("DELETE FROM some_table");
        ///         transaction.Commit();
        ///     }
        /// </code></example>
        [PublicAPI, NotNull]
        public static IDbConnection OpenNow([NotNull] this IDbConnection connection)
        {
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Executes the specified SQL statement, with optional parameters.
        /// </summary>
        /// <param name="connection">
        /// The <see cref="IDbConnection"/> on which to execute the SQL statement.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="parameters">
        /// Optional parameters to the SQL statement. Leave <c>null</c> if there are no parameters.
        /// </param>
        /// <returns>
        /// The number of rows affected
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="connection"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="sql"/> is <c>null</c>.</para>
        /// </exception>
        /// <seealso cref="IDbCommand.ExecuteNonQuery"/>.
        /// <example><code>
        ///     IDbConnection connection = ...
        ///     int rowsAffected = connection.Execute("INSERT INTO some_table VALUES (@key, @value)", new
        ///     {
        ///         key = "Meaning of Life",
        ///         value = 42
        ///     });
        /// </code></example>
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

        /// <summary>
        /// Queries the database for a list of rows and maps them to new instances of an object,
        /// mapping each column to the corresponding property on the object and assigns values from
        /// the rows into the objects.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to return for each row in the result set.
        /// </typeparam>
        /// <param name="connection">
        /// The <see cref="IDbConnection"/> on which to execute the SQL statement.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="parameters">
        /// Optional parameters to the SQL statement. Leave <c>null</c> if there are no parameters.
        /// </param>
        /// <returns>
        /// A <see cref="List{T}"/> of <typeparamref name="T"/> holding one object per row in the
        /// result set from the database.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="connection"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="sql"/> is <c>null</c>.</para>
        /// </exception>
        /// <seealso cref="IDbCommand.ExecuteReader()"/>.
        /// <example><code>
        ///     public class Record
        ///     {
        ///         public string Key { get; set; }
        ///         public int Value { get; set; }
        ///     }
        ///     ...
        ///     IDbConnection connection = ...
        ///     var result = connection.Query&lt;Record&gt;("SELECT Key, Value FROM some_table");
        /// </code></example>
        /// <remarks>
        /// Note that <see cref="Query{T}(IDbConnection,string,object)"/> must be able to construct
        /// new instances of <typeparamref name="T"/> so a parameterless public constructor is
        /// required.
        /// </remarks>
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

        /// <summary>
        /// Queries the database for a list of rows and maps them to new instances of an object,
        /// using the constructor of the object to construct new objects and assign values
        /// to it, mapping columns from the result set to the parameters to the constructor.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to return for each row in the result set.
        /// </typeparam>
        /// <param name="connection">
        /// The <see cref="IDbConnection"/> on which to execute the SQL statement.
        /// </param>
        /// <param name="sql">
        /// The SQL statement to execute.
        /// </param>
        /// <param name="parameters">
        /// Optional parameters to the SQL statement. Leave <c>null</c> if there are no parameters.
        /// </param>
        /// <returns>
        /// A <see cref="List{T}"/> of <typeparamref name="T"/> holding one object per row in the
        /// result set from the database.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="connection"/> is <c>null</c>.</para>
        /// <para>- or -</para>
        /// <para><paramref name="sql"/> is <c>null</c>.</para>
        /// </exception>
        /// <seealso cref="IDbCommand.ExecuteReader()"/>.
        /// <example><code>
        ///     IDbConnection connection = ...
        ///     var result = connection.Query("SELECT Key, Value FROM some_table", new
        ///     {
        ///         Key = "dummy key",
        ///         Value = 0
        ///     });
        ///     if (result.Any(r => r.Key == "Meaning of Life"))
        ///         Debug.WriteLine("Success!");
        /// </code></example>
        /// <remarks>
        /// While this method is meant primarily for anonymous objects, you can use any type of
        /// object as long as it has a public constructor with parameters that has names that can
        /// be matched with column names from the result set.
        /// </remarks>
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
        public static T QueryScalar<T>([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            using (var command = connection.PrepareForQueryScalar<T>(sql, parameters))
            {
                return command.Execute();
            }
        }

        [PublicAPI, NotNull]
        public static IDbPreparedCommand<T> PrepareForQueryScalar<T>([NotNull] this IDbConnection connection, [NotNull] string sql, [CanBeNull] object parameters = null)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (sql == null)
                throw new ArgumentNullException("sql");

            return new DbPreparedQueryScalar<T>(connection, null, sql, parameters);
        }
    }
}