using System;
using System.Data;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute

namespace Sequel.Tests
{
    [TestFixture]
    public class DbConnectionQueryTests
    {
        [Test]
        public void Query_NullConnection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DbConnectionExtensions.Query<TestRecord>(null, "SQL"));
        }

        [Test]
        public void Query_NullSql_ThrowsArgumentNullException()
        {
            var connection = Substitute.For<IDbConnection>();
            Assert.Throws<ArgumentNullException>(() => connection.Query<TestRecord>(null));
        }

        [Test]
        public void Query_WithSql_ReturnsListOfRecords()
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

            var result = connection.Query<TestRecord>("SQL");

            CollectionAssert.AreEqual(new[]
            {
                new TestRecord { Key = "Magic", Value = 42 },
            }, result);
        }
    }
}
