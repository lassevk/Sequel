using NUnit.Framework;

namespace Sequel.SQLiteTests.cs
{
    [TestFixture]
    public class SQLiteExecuteTests : SQLiteTestBase
    {
        [Test]
        public void QueryScalar_OnCreatedTableWithOneRow_ReturnsOne()
        {
            Connection.Execute("CREATE TABLE some_table (Key TEXT, Value INTEGER)");
            Connection.Execute("INSERT INTO some_table VALUES (@Key, @Value)", new { Key = "Meaning of Life", Value = 42 });
            var rowCount = Connection.QueryScalar<int>("SELECT COUNT(*) FROM some_table");

            Assert.That(rowCount, Is.EqualTo(1));
        }

        [Test]
        public void QuerySequenceForFirstColumn_OnCreatedTableWithTwoRows_ReturnsTheValuesOfTheFirstColumn()
        {
            Connection.Execute("CREATE TABLE some_table (Key TEXT, Value INTEGER)");
            using (var insert = Connection.PrepareForExecute("INSERT INTO some_table VALUES (@Key, @Value)", new { Key = "", Value = 0 }))
            {
                insert.Execute(new { Key = "Meaning of Life 1", Value = 42 });
                insert.Execute(new { Key = "Meaning of Life 2", Value = 84 });
            }

            var rows = Connection.QuerySequence<string>("SELECT Key FROM some_table ORDER BY Key");

            CollectionAssert.AreEqual(new[]
            {
                "Meaning of Life 1",
                "Meaning of Life 2"
            }, rows);
        }

        [Test]
        public void QuerySequenceForSecondColumn_OnCreatedTableWithTwoRows_ReturnsTheValuesOfTheSecondColumn()
        {
            Connection.Execute("CREATE TABLE some_table (Key TEXT, Value INTEGER)");
            using (var insert = Connection.PrepareForExecute("INSERT INTO some_table VALUES (@Key, @Value)", new { Key = "", Value = 0 }))
            {
                insert.Execute(new { Key = "Meaning of Life 1", Value = 42 });
                insert.Execute(new { Key = "Meaning of Life 2", Value = 84 });
            }

            var rows = Connection.QuerySequence<int>("SELECT Value FROM some_table ORDER BY Key");

            CollectionAssert.AreEqual(new[]
            {
                42,
                84
            }, rows);
        }
    }
}
