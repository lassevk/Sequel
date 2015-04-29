using System;
using System.Data;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute

namespace Sequel.Tests
{
    [TestFixture]
    public class DbConnectionQuerySequenceTests
    {
        [Test]
        public void QuerySequence_NullConnection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DbConnectionExtensions.QuerySequence<int>(null, "SQL"));
        }

        [Test]
        public void QuerySequence_NullSql_ThrowsArgumentNullException()
        {
            var connection = Substitute.For<IDbConnection>();
            Assert.Throws<ArgumentNullException>(() => connection.QuerySequence<int>(null));
        }

        [Test]
        public void QuerySequence_WithSql_ReturnsListOfRecords()
        {
            var connection = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connection.CreateCommand().Returns(command);

            IDataReader reader = Substitute.For<IDataReader>();
            command.ExecuteReader().Returns(reader);

            reader.Read().Returns(ci => true, ci => false);
            reader.FieldCount.Returns(2);
            reader.GetName(0).Returns("Value");
            reader.GetValue(0).Returns(42);

            var result = connection.QuerySequence<int>("SQL");

            CollectionAssert.AreEqual(new[]
            {
                42
            }, result);
        }
    }
}