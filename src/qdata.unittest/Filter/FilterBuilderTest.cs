using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace RoyLab.QData.Filter
{
    public class FilterBuilderTest
    {
        [Test]
        public void TestExpression()
        {
            var function = FilterParser.Parse("Name=roy").Build(typeof(User))?.Compile();
            Assert.IsInstanceOf<Func<User, bool>>(function);
            var filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);
            Assert.IsTrue(filterFunction(new User {Name = "roy"}));
            Assert.IsFalse(filterFunction(new User {Name = "royzhang666"}));

            function = FilterParser.Parse("|(Name=roy)(Name=royzhang666)")
                .Build(typeof(User))
                ?.Compile();
            Assert.IsInstanceOf<Func<User, bool>>(function);
            filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);
            Assert.IsTrue(filterFunction(new User {Name = "roy"}));
            Assert.IsTrue(filterFunction(new User {Name = "royzhang666"}));
            Assert.IsFalse(filterFunction(new User {Name = "royzhang"}));

            function = FilterParser.Parse("&(Age>=18)(Age<35)").Build(typeof(User))?.Compile();
            Assert.IsInstanceOf<Func<User, bool>>(function);
            filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);
            Assert.IsFalse(filterFunction(new User {Age = 17}));
            Assert.IsTrue(filterFunction(new User {Age = 18}));
            Assert.IsTrue(filterFunction(new User {Age = 34}));
            Assert.IsFalse(filterFunction(new User {Age = 35}));

            function = FilterParser.Parse("|(Age>=18)(Age<35)").Build(typeof(User))?.Compile();
            Assert.IsInstanceOf<Func<User, bool>>(function);
            filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);
            Assert.IsTrue(filterFunction(new User {Age = 17}));
            Assert.IsTrue(filterFunction(new User {Age = 18}));
            Assert.IsTrue(filterFunction(new User {Age = 34}));
            Assert.IsTrue(filterFunction(new User {Age = 35}));

            function = FilterParser.Parse("Location=2").Build(typeof(User))?.Compile();
            Assert.IsInstanceOf<Func<User, bool>>(function);
            filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);
            Assert.IsTrue(filterFunction(new User {Location = Location.SanFrancisco}));
            Assert.IsFalse(filterFunction(new User {Location = Location.NewYork}));

            var lambdaExpression = FilterParser.Parse("Age in [10,18]").Build(typeof(User));
            function = lambdaExpression?.Compile();
            Assert.IsInstanceOf<Func<User, bool>>(function);
            filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);
            Assert.IsTrue(filterFunction(new User {Age = 10}));
            Assert.IsTrue(filterFunction(new User {Age = 18}));
            Assert.IsFalse(filterFunction(new User {Age = 34}));

            lambdaExpression = FilterParser.Parse("Location in [1,4]").Build(typeof(User));
            function = lambdaExpression?.Compile();
            Assert.IsInstanceOf<Func<User, bool>>(function);
            filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);
            Assert.IsTrue(filterFunction(new User {Location = Location.NewYork}));
            Assert.IsFalse(filterFunction(new User {Location = Location.SanFrancisco}));

            lambdaExpression = FilterParser.Parse("&(BirthDay=5/22/2015 12:00:00 PM)(Age<18)").Build(typeof(User));
            function = lambdaExpression?.Compile();
            Assert.IsInstanceOf<Func<User, bool>>(function);
            filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);
            Assert.IsTrue(filterFunction(new User
            {
                BirthDay = new DateTime(2015, 5, 22, 12, 0, 0),
                Age = 5
            }));

            lambdaExpression = FilterParser.Parse("UserID=9b4959dc-45b4-45c7-8e6d-536bc770ace0").Build(typeof(User));
            function = lambdaExpression?.Compile();
            Assert.IsInstanceOf<Func<User, bool>>(function);
            filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);
            Assert.IsTrue(filterFunction(new User {UserID = Guid.Parse("9b4959dc-45b4-45c7-8e6d-536bc770ace0")}));
            Assert.IsFalse(filterFunction(new User()));
        }

        [Test]
        public void TestInExpression()
        {
            var lambdaExpression = FilterParser.Parse("UserID in [9b4959dc-45b4-45c7-8e6d-536bc770ace0]")
                .Build(typeof(User));
            var function = lambdaExpression.Compile();
            var filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);

            Assert.IsTrue(filterFunction(new User {UserID = Guid.Parse("9b4959dc-45b4-45c7-8e6d-536bc770ace0")}));
            Assert.IsFalse(filterFunction(new User()));
        }

        [Test]
        public void TestInExpressionWithEmptySet()
        {
            var lambdaExpression = FilterParser.Parse("UserID in []").Build(typeof(User));
            var function = lambdaExpression.Compile();
            var filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);

            Assert.IsFalse(filterFunction(new User {UserID = Guid.Parse("9b4959dc-45b4-45c7-8e6d-536bc770ace0")}));
            Assert.IsTrue(filterFunction(new User()));
        }

        [Test]
        public void TestExpression2()
        {
            Expression<Func<User, bool>> expression = p => new[] {10, 20, 30}.Contains(p.Age);
            var func = expression.Compile();
        }
    }
}