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
            var parser = TypeUtility.GetTryParseFunction(typeof(DateTime));
            var dateTime = parser.DynamicInvoke($"{DateTime.Now}");
            Console.WriteLine(dateTime);
        }
    }
}