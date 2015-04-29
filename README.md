# Sequel

Sequel aims to add extension methods to [IDbConnection][idbc] and [IDbTransaction][idbt]
to facilitate easier execution of SQL through the underlying connection providers.

Here is a list of extension methods added (note that for brevity I have omitted the implicit `this IDbConnection connection`
or `this IDbTransaction transaction` parameters.

* `int Execute(string sql, object parameters = null)`
* `List<T> Query<T>(string sql, object parameters = null)`
* `List<T> QuerySequence(string sql, object parameters = null)`
* `List<T> QueryAnonymous<T>(string sql, T template, object parameters = null)`
* `T? QueryScalar<T>(string sql, object parameters = null)`
* `string QueryStringScalar(string sql, object parameters = null)`

Additionally there are `PrepareForX` methods for each of the above which returns an object implementing the [IDbPreparedCommand<T>](idbp)
interface that has this single method:

    T Execute(object parameterValues = null);
    
Where `T` here corresponds to the matching method in the list above. Example: `PrepareForQuery<T>` returns
`IDbPreparedCommand<List<T>>`.

The methods in the list above executes directly, returning the results. The `PrepareFor` variants returns an object where
the reflection work that can be done up front is cached and reused. These are meant to be used in a context where
you intend to execute the same statements over and over again, potentially with different parameter values.

# Specifying parameters

Specifying parameters is done using a wrapper object instead of an array of some parameter object.

You basically wrap up the parameter values in an object with properties where the names and types and current values
of the properties are used to add parameters to the command.

See the example below for what this looks like.

# Example of usage with SQLite

    using (var connection = new SQLiteConnection(@"Data Source=D:\Temp\Test.sqlite3"))
    {
        connection.Open();
        using (var transaction = connection.BeginTransaction())
        {
            transaction.Execute("delete from some_table where key = @key", new { key = 42 });
            transaction.Execute("insert into some_table (key, value) values (@key, @value)", new {
                key = "mol",
                value = 42
            });
            var records = transaction.QueryAnonymous("select * from some_table", new { key = "dummy", value = 0 });
            if (records.Any(r => r.value == 42))
                Debug.WriteLine("SUCCESS!");
            transaction.Commit();
        }
    }

  [idbc]: https://msdn.microsoft.com/en-us/library/system.data.idbconnection%28v=vs.110%29.aspx
  [idbt]: https://msdn.microsoft.com/en-us/library/system.data.idbtransaction%28v=vs.110%29.aspx
  [idbp]: https://github.com/lassevk/Sequel/blob/master/Sequel/IDbPreparedCommand.cs
