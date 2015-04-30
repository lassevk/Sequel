using System.Data.SQLite;
using System.IO;
using NUnit.Framework;

namespace Sequel.SQLiteTests.cs
{
    public class SQLiteTestBase
    {
        private string _FileName;
        private SQLiteConnection _Connection;

        [SetUp]
        public void SetUp()
        {
            _FileName = Path.GetTempFileName();
            _Connection = new SQLiteConnection("Data Source=" + _FileName).OpenNow();
        }

        [TearDown]
        public void TearDown()
        {
            _Connection.Dispose();
            if (File.Exists(_FileName))
                File.Delete(_FileName);
        }

        protected SQLiteConnection Connection
        {
            get
            {
                return _Connection;
            }
        }
    }
}
