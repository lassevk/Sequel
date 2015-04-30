using System;
using System.Data;
using NSubstitute;
using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute

namespace Sequel.Tests
{
    [TestFixture]
    public class OpenNowTests
    {
        [Test]
        public void OpenNow_NullConnection_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => DbConnectionExtensions.OpenNow<IDbConnection>(null));
        }

        [Test]
        public void OpenNow_WithConnection_CallsOpenOnConnection()
        {
            var connection = Substitute.For<IDbConnection>();
            
            connection.OpenNow();

            connection.Received().Open();
        }
    }
}
