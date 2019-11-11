using System.Linq;
using NUnit.Framework;
using RoyLab.QData.Lite.Filter.Expressions;

namespace RoyLab.QData.Lite.Filter
{
    internal class TestFilterParser
    {
        [Test]
        public void TestBasic()
        {
            var success = FilterParser.TryParse("", out var parsedExpression, out var remaining);
            Assert.IsTrue(success);
            Assert.IsNull(parsedExpression);
            Assert.IsTrue(remaining.IsEmpty);

            success = FilterParser.TryParse("   ", out parsedExpression, out remaining);
            Assert.IsTrue(success);
            Assert.IsNull(parsedExpression);
            Assert.IsTrue(remaining.IsEmpty);

            success = FilterParser.TryParse("()", out parsedExpression, out remaining);
            Assert.IsFalse(success);
            Assert.IsNull(parsedExpression);
            Assert.IsTrue(remaining.IsEmpty);

            success = FilterParser.TryParse("  (  (  )  ) ", out parsedExpression, out remaining);
            Assert.IsFalse(success);
            Assert.IsNull(parsedExpression);
            Assert.IsTrue(remaining.IsEmpty);

            success = FilterParser.TryParse("  (   ) ( ) ", out parsedExpression, out remaining);
            Assert.IsFalse(success);
            Assert.IsNull(parsedExpression);
            Assert.AreEqual(") ", remaining.ToString());

            success = FilterParser.TryParse("a", out parsedExpression, out remaining);
            Assert.IsFalse(success);
            Assert.IsNull(parsedExpression);
            Assert.AreEqual(string.Empty, remaining.ToString());
        }

        [TestCase("&(a = 1)( b>12) ")]
        [TestCase("&(a = 1) ( b>12) ")]
        [TestCase("(&(a = 1) ( b>12) )")]
        [TestCase("((&((a = 1)) ( b>12) ))  ")]
        [TestCase("((&(((((a = 1)) ) )) (((( b>12) )))))  ")]
        public void TestBasicAndExpression(string filter)
        {
            var success = FilterParser.TryParse(filter, out var parsedExpression, out var remaining);
            Assert.IsTrue(success);
            Assert.IsTrue(remaining.IsEmpty);
            Assert.IsInstanceOf<AndExpression>(parsedExpression);
            var left = ((AndExpression) parsedExpression).Left as CompareExpression;
            Assert.IsNotNull(left);
            Assert.AreEqual("a", left.Variable);
            Assert.AreEqual("1", left.Value);
            Assert.AreEqual(Operation.Eq, left.Operation);
            var right = ((AndExpression) parsedExpression).Right as CompareExpression;
            Assert.IsNotNull(right);
            Assert.AreEqual("b", right.Variable);
            Assert.AreEqual("12", right.Value);
            Assert.AreEqual(Operation.Gt, right.Operation);
        }

        [TestCase("!(a= 12)")]
        [TestCase("!((a= 12))")]
        [TestCase("((!((a= 12)) )) ")]
        public void TestBasicNotExpression(string filter)
        {
            var success = FilterParser.TryParse(filter, out var parsedExpression, out var remaining);
            Assert.IsTrue(success);
            Assert.IsTrue(remaining.IsEmpty);
            Assert.IsInstanceOf<NotExpression>(parsedExpression);
            var notExpression = parsedExpression as NotExpression;
            Assert.IsNotNull(notExpression);
            Assert.IsInstanceOf<CompareExpression>(notExpression.Single);
            var singleExpression = notExpression.Single as CompareExpression;
            Assert.IsNotNull(singleExpression);
            Assert.AreEqual("a", singleExpression.Variable);
            Assert.AreEqual(Operation.Eq, singleExpression.Operation);
            Assert.AreEqual("12", singleExpression.Value);
        }

        [TestCase("|(a = 1)( b>12) ")]
        [TestCase("|(a = 1) ( b>12) ")]
        [TestCase("(|(a = 1) ( b>12) )")]
        [TestCase("((|((a = 1)) ( b>12) ))  ")]
        [TestCase("((|(((((a = 1)) ) )) (((( b>12) )))))  ")]
        public void TestBasicOrExpression(string filter)
        {
            var success = FilterParser.TryParse(filter, out var parsedExpression, out var remaining);
            Assert.IsTrue(success);
            Assert.IsTrue(remaining.IsEmpty);
            Assert.IsInstanceOf<OrExpression>(parsedExpression);
            var left = ((OrExpression) parsedExpression).Left as CompareExpression;
            Assert.IsNotNull(left);
            Assert.AreEqual("a", left.Variable);
            Assert.AreEqual("1", left.Value);
            Assert.AreEqual(Operation.Eq, left.Operation);
            var right = ((OrExpression) parsedExpression).Right as CompareExpression;
            Assert.IsNotNull(right);
            Assert.AreEqual("b", right.Variable);
            Assert.AreEqual("12", right.Value);
            Assert.AreEqual(Operation.Gt, right.Operation);
        }

        [TestCase("a=12", "a", Operation.Eq, "12")]
        [TestCase("a>12", "a", Operation.Gt, "12")]
        [TestCase("a>=12", "a", Operation.Ge, "12")]
        [TestCase("a<12", "a", Operation.Lt, "12")]
        [TestCase("a<=12", "a", Operation.Le, "12")]
        [TestCase("   a=12", "a", Operation.Eq, "12")]
        [TestCase("   a   =12", "a", Operation.Eq, "12")]
        [TestCase("   a   =   12", "a", Operation.Eq, "12")]
        [TestCase("   a   =   12   ", "a", Operation.Eq, "12")]
        [TestCase("(a=12)", "a", Operation.Eq, "12")]
        [TestCase("((a=12))", "a", Operation.Eq, "12")]
        public void TestBasicCompareExpression(string filter, string variable, Operation operation, string value)
        {
            var success = FilterParser.TryParse(filter, out var parsedExpression, out var remaining);
            Assert.IsTrue(success);
            Assert.IsTrue(remaining.IsEmpty);
            Assert.IsInstanceOf<CompareExpression>(parsedExpression);
            var compareExpression = parsedExpression as CompareExpression;
            Assert.IsNotNull(compareExpression);
            Assert.AreEqual(variable, compareExpression.Variable);
            Assert.AreEqual(operation, compareExpression.Operation);
            Assert.AreEqual(value, compareExpression.Value);
        }

        [TestCase("|&(a = 1) ( b>12) (c= 128)")]
        [TestCase("|(&(a = 1) ( b>12) )(c= 128)")]
        [TestCase("(|(&(a = 1) ( b>12) )(c= 128))")]
        public void TestComplexLogicalExpressionCase1(string filter)
        {
            var success = FilterParser.TryParse(filter, out var parsedExpression, out var remaining);
            Assert.IsTrue(success);
            Assert.IsTrue(remaining.IsEmpty);
            Assert.IsNotNull(parsedExpression);
            Assert.IsInstanceOf<OrExpression>(parsedExpression);
            var orExpression = parsedExpression as OrExpression;
            Assert.IsNotNull(orExpression);
            Assert.IsInstanceOf<AndExpression>(orExpression.Left);
            var leftExpression = orExpression.Left as AndExpression;
            Assert.IsNotNull(leftExpression);
            Assert.IsInstanceOf<CompareExpression>(leftExpression.Left);
            var firstCompareExpression = leftExpression.Left as CompareExpression;
            Assert.IsNotNull(firstCompareExpression);
            Assert.AreEqual("a", firstCompareExpression.Variable);
            Assert.AreEqual(Operation.Eq, firstCompareExpression.Operation);
            Assert.AreEqual("1", firstCompareExpression.Value);
            Assert.IsInstanceOf<CompareExpression>(leftExpression.Right);
            var secondCompareExpression = leftExpression.Right as CompareExpression;
            Assert.IsNotNull(secondCompareExpression);
            Assert.AreEqual("b", secondCompareExpression.Variable);
            Assert.AreEqual(Operation.Gt, secondCompareExpression.Operation);
            Assert.AreEqual("12", secondCompareExpression.Value);
            Assert.IsInstanceOf<CompareExpression>(orExpression.Right);
            var thirdCompareExpression = orExpression.Right as CompareExpression;
            Assert.IsNotNull(thirdCompareExpression);
            Assert.AreEqual("c", thirdCompareExpression.Variable);
            Assert.AreEqual(Operation.Eq, thirdCompareExpression.Operation);
            Assert.AreEqual("128", thirdCompareExpression.Value);
        }

        [TestCase("&!(a=1)(b=2)")]
        [TestCase("&(!(a=1))(b=2)")]
        [TestCase("&(!(((a=1) )) )(b =2)")]
        [TestCase("((&(!(a=1 )) (b=2 )))")]
        [TestCase("((&(!(a = 1)) (((b =2)))))")]
        public void TestComplexLogicalExpressionCase2(string filter)
        {
            var success = FilterParser.TryParse(filter, out var parsedExpression, out var remaining);
            Assert.IsTrue(success);
            Assert.IsTrue(remaining.IsEmpty);
            Assert.IsInstanceOf<AndExpression>(parsedExpression);
            var andExpression = parsedExpression as AndExpression;
            Assert.IsNotNull(andExpression);
            var left = andExpression.Left as NotExpression;
            Assert.IsNotNull(left);
            var firstCompareExpression = left.Single as CompareExpression;
            Assert.IsNotNull(firstCompareExpression);
            Assert.AreEqual("a", firstCompareExpression.Variable);
            Assert.AreEqual(Operation.Eq, firstCompareExpression.Operation);
            Assert.AreEqual("1", firstCompareExpression.Value);
            var right = andExpression.Right as CompareExpression;
            Assert.IsNotNull(right);
            Assert.AreEqual("b", right.Variable);
            Assert.AreEqual(Operation.Eq, right.Operation);
            Assert.AreEqual("2", right.Value);
        }

        [TestCase("(!(&(a = 1) ( b>12) )(c= 128))", "(c= 128))")]
        [TestCase("((&((a = 1)) ( b>=12) ))(a=11)", "(a=11)")]
        [TestCase("&(a=1)(b=2)(c=3)", "(c=3)")]
        [TestCase("|(a=1)(b=2)(c=3)", "(c=3)")]
        [TestCase("!(a=1)(b=2)", "(b=2)")]
        public void TestComplexLogicalExpressionInvalidCases(string filter, string expectedRemaining)
        {
            var success = FilterParser.TryParse(filter, out _, out var remaining);
            Assert.IsFalse(success);
            Assert.AreEqual(expectedRemaining, remaining.ToString());
        }

        [TestCase("a in [1]", "a", "1")]
        [TestCase("a in [1,2,3,4,5]", "a", "1", "2", "3", "4", "5")]
        [TestCase("a in [ 1, 2, 3, 4, 5   ]   ", "a", "1", "2", "3", "4", "5")]
        [TestCase("a in [12 3]", "a", "12 3")]
        [TestCase("a in [12 3,4,5]", "a", "12 3", "4", "5")]
        [TestCase("a in [ 1 , ]", "a", "1", "")]
        [TestCase("a in [ , , ]", "a", "", "", "")]
        public void TestInExpression(string filter, string variable, params string[] values)
        {
            var success = FilterParser.TryParse(filter, out var parsedExpression, out var remaining);
            Assert.IsTrue(success);
            Assert.IsTrue(remaining.IsEmpty);
            Assert.IsInstanceOf<InExpression>(parsedExpression);
            var inExpression = parsedExpression as InExpression;
            Assert.IsNotNull(inExpression);
            Assert.AreEqual(variable, inExpression.Variable);
            Assert.IsTrue(values.SequenceEqual(inExpression.ValueList), string.Join(",", inExpression.ValueList));
        }

        [TestCase("a in 1,2,3", "1,2,3")]
        [TestCase("a in [ 1 , 2 ,3", "")]
        public void TestInExpressionInvalidCases(string filter, string expectedRemaining)
        {
            var success = FilterParser.TryParse(filter, out var parsedExpression, out var remaining);
            Assert.IsFalse(success);
            Assert.IsNull(parsedExpression);
            Assert.AreEqual(expectedRemaining, remaining.ToString(), remaining.ToString());
        }
    }
}