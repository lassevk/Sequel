using System;
using System.Data;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute

namespace Sequel.Tests
{
    [TestFixture]
    public class DbConnectionQueryAnonymousTests
    {
        [Test]
        public void QueryAnonymous_NullConnection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DbConnectionExtensions.QueryAnonymous<TestRecord>(null, "SQL"));
        }

        [Test]
        public void QueryAnonymous_NullSql_ThrowsArgumentNullException()
        {
            var connection = Substitute.For<IDbConnection>();
            Assert.Throws<ArgumentNullException>(() => connection.QueryAnonymous<TestRecord>(null));
        }

        [Test]
        public void QueryAnonymous_WithSql_ReturnsListOfRecords()
        {
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            IDataReader reader = Substitute.For<IDataReader>();
            command.ExecuteReader().Returns(reader);

            reader.Read().Returns(ci => true, ci => false);
            reader.FieldCount.Returns(2);
            reader.GetName(0).Returns("Key");
            reader.GetName(1).Returns("Value");
            reader.GetValue(0).Returns("Magic");
            reader.GetValue(1).Returns(42);

            var result = connection.QueryAnonymous("SQL", new { Key = "", Value = 0 });

            CollectionAssert.AreEqual(new[]
            {
                new { Key = "Magic", Value = 42 },
            }, result);
        }
    }
}
