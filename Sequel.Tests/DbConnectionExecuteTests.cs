using System;
using System.Data;
using NSubstitute;
using NUnit.Framework;

namespace Sequel.Tests
{
    [TestFixture]
    public class DbConnectionExecuteTests
    {
        [Test]
        public void PrepareForExecute_NullConnection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DbConnectionExtensions.Execute(null, "SQL"));
        }

        [Test]
        public void PrepareForExecute_NullSql_ThrowsArgumentNullException()
        {
            var connection = Substitute.For<IDbConnection>();
            Assert.Throws<ArgumentNullException>(() => connection.Execute(null));
        }

        [Test]
        public void Execute_WithConnection_ExecutesSql()
        {
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            connection.Execute("SQL");

            Assert.That(command.CommandText, Is.EqualTo("SQL"));
            command.Received().ExecuteNonQuery();
        }

        [Test]
        public void Execute_WithParameters_AssignsParameterValues()
        {
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            var parameterCollection = Substitute.For<IDataParameterCollection>();
            IDataParameter addedParameter = null;
            parameterCollection.WhenForAnyArgs(collection => collection.Add(null)).Do(ci => addedParameter = (IDataParameter)ci[0]);

            command.Parameters.Returns(parameterCollection);
            connection.CreateCommand().Returns(command);

            connection.Execute("SQL", new { magic = 42 });

            Assert.That(command.CommandText, Is.EqualTo("SQL"));

            command.Received().ExecuteNonQuery();
            parameterCollection.ReceivedWithAnyArgs().Add(null);

            Assert.That(addedParameter, Is.Not.Null);
            Assert.That(addedParameter.ParameterName, Is.EqualTo("magic"));
            Assert.That(addedParameter.Value, Is.EqualTo(42));
        }
    }
}
