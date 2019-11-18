using NUnit.Framework;

namespace RoyLab.QData.Updater
{
    public class TestUpdaterParser
    {
        [Test]
        public void TestBasicParse()
        {
            var success = UpdaterParser.TryParse("Name", out _, out _);
            Assert.IsFalse(success);
            success = UpdaterParser.TryParse("Name=roy;Age", out _, out _);
            Assert.IsFalse(success);
            success = UpdaterParser.TryParse("Name=roy;;", out _, out _);
            Assert.IsFalse(success);

            success = UpdaterParser.TryParse("Name=", out var parsedExpressions, out var valueArray);
            Assert.IsTrue(success);
            Assert.AreEqual(1, parsedExpressions.Count);
            Assert.AreEqual(2, valueArray.Length);
            Assert.AreEqual("Name", parsedExpressions[0].Variable);
            Assert.AreEqual(1, parsedExpressions[0].Index);
            Assert.AreEqual("", valueArray[1]);

            success = UpdaterParser.TryParse("Name=roy", out parsedExpressions, out valueArray);
            Assert.IsTrue(success);
            Assert.AreEqual(1, parsedExpressions.Count);
            Assert.AreEqual(2, valueArray.Length);
            Assert.AreEqual("Name", parsedExpressions[0].Variable);
            Assert.AreEqual(1, parsedExpressions[0].Index);
            Assert.AreEqual("roy", valueArray[1]);

            success = UpdaterParser.TryParse("Name=roy;", out parsedExpressions, out valueArray);
            Assert.IsTrue(success);
            Assert.AreEqual(1, parsedExpressions.Count);
            Assert.AreEqual(2, valueArray.Length);
            Assert.AreEqual("Name", parsedExpressions[0].Variable);
            Assert.AreEqual(1, parsedExpressions[0].Index);
            Assert.AreEqual("roy", valueArray[1]);

            success = UpdaterParser.TryParse("Name=roy;Age=", out parsedExpressions, out valueArray);
            Assert.IsTrue(success);
            Assert.AreEqual(2, parsedExpressions.Count);
            Assert.AreEqual(3, valueArray.Length);
            Assert.AreEqual("Name", parsedExpressions[0].Variable);
            Assert.AreEqual(1, parsedExpressions[0].Index);
            Assert.AreEqual("roy", valueArray[1]);
            Assert.AreEqual("Age", parsedExpressions[1].Variable);
            Assert.AreEqual(2, parsedExpressions[1].Index);
            Assert.AreEqual("", valueArray[2]);
        }

        [Test]
        public void TestParseWithEscapes()
        {
            var success = UpdaterParser.TryParse(@"Name=\;", out var parsedExpressions, out var valueArray);
            Assert.IsTrue(success);
            Assert.AreEqual(1, parsedExpressions.Count);
            Assert.AreEqual(2, valueArray.Length);
            Assert.AreEqual("Name", parsedExpressions[0].Variable);
            Assert.AreEqual(1, parsedExpressions[0].Index);
            Assert.AreEqual(@";", valueArray[1]);

            success = UpdaterParser.TryParse(@"Name=\\;", out parsedExpressions, out valueArray);
            Assert.IsTrue(success);
            Assert.AreEqual(1, parsedExpressions.Count);
            Assert.AreEqual(2, valueArray.Length);
            Assert.AreEqual("Name", parsedExpressions[0].Variable);
            Assert.AreEqual(1, parsedExpressions[0].Index);
            Assert.AreEqual(@"\", valueArray[1]);

            success = UpdaterParser.TryParse(@"Name=\\\;", out parsedExpressions, out valueArray);
            Assert.IsTrue(success);
            Assert.AreEqual(1, parsedExpressions.Count);
            Assert.AreEqual(2, valueArray.Length);
            Assert.AreEqual("Name", parsedExpressions[0].Variable);
            Assert.AreEqual(1, parsedExpressions[0].Index);
            Assert.AreEqual(@"\;", valueArray[1]);
        }
    }
}