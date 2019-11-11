using System;
using NUnit.Framework;

namespace RoyLab.QData.Lite.Filter
{
    public class TestFilterBuilder
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

            function = FilterParser.Parse("Age in [10,18]").Build(typeof(User))?.Compile();
            Assert.IsInstanceOf<Func<User, bool>>(function);
            filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);
            Assert.IsTrue(filterFunction(new User {Age = 10}));
            Assert.IsTrue(filterFunction(new User {Age = 18}));
            Assert.IsFalse(filterFunction(new User {Age = 34}));

            function = FilterParser.Parse("Location in [1,4]").Build(typeof(User))?.Compile();
            Assert.IsInstanceOf<Func<User, bool>>(function);
            filterFunction = function as Func<User, bool>;
            Assert.IsNotNull(filterFunction);
            Assert.IsTrue(filterFunction(new User {Location = Location.NewYork}));
            Assert.IsFalse(filterFunction(new User {Location = Location.SanFrancisco}));
        }
    }
}