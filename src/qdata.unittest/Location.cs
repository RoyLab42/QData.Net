using System;
using NUnit.Framework;

namespace RoyLab.QData
{
    public enum Location
    {
        NewYork = 1,
        SanFrancisco = 2
    }

    [TestFixture]
    public class LocationTest
    {
        [Test]
        public void Test1()
        {
            var parsedLocation = Enum.Parse<Location>("2");
            Assert.AreEqual(Location.SanFrancisco, parsedLocation);

            parsedLocation = Enum.Parse<Location>("SanFrancisco");
            Assert.AreEqual(Location.SanFrancisco, parsedLocation);

            try
            {
                parsedLocation = Enum.Parse<Location>("xxx");
                Assert.Fail("should throw exception");
            }
            catch (Exception e)
            {
                Assert.Pass("expected exception: {0}", e);
            }

            var success = Enum.TryParse<Location>("xxx", out parsedLocation);
            Assert.IsFalse(success);
            Assert.AreEqual(0, parsedLocation);
        }
    }
}