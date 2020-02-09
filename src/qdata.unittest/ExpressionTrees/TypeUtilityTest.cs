using System;
using NUnit.Framework;
using RoyLab.QData.Converters.ExpressionTrees;

namespace RoyLab.QData.ExpressionTrees
{
    public class TypeUtilityTest
    {
        [Test]
        public void TestBuildTryParse()
        {
            DateTime.TryParse("2020-01-01 12:22:30", out var dateTime);
            var dateTimeParser = TypeUtility.GetTryParseFunction(typeof(DateTime));
            var parsedDateTime = dateTimeParser.DynamicInvoke($"{dateTime}");
            Assert.AreEqual(dateTime, parsedDateTime);

            var intParser = TypeUtility.GetTryParseFunction(typeof(int));
            var parsedInt = intParser.DynamicInvoke("123");
            Assert.IsNotNull(parsedInt);
            Assert.AreEqual(123, parsedInt);
        }
    }
}