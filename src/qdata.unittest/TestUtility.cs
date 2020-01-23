using NUnit.Framework;
using System;
using System.Linq;
using System.Text.Json;

namespace RoyLab.QData
{
    public class Tests
    {
        private User[] users;

        [SetUp]
        public void Initialize()
        {
            users = new[]
            {
                new User {Name = "Alice", Age = 10, Location = Location.SanFrancisco},
                new User {Name = "Bob", Age = 18, Location = Location.NewYork},
                new User {Name = "Roy", Age = 24, Location = Location.SanFrancisco},
                new User {Name = "Jack", Age = 30, Location = Location.NewYork}
            };
        }

        [Test]
        public void TestFilter()
        {
            var resultSet = users.AsQueryable()
                .QueryDynamic(null, "Age<20")
                .OfType<object>()
                .ToList();
            Assert.AreEqual(2, resultSet.Count);
            Assert.AreSame(users[0], resultSet[0]);
            Assert.AreSame(users[1], resultSet[1]);

            resultSet = users.AsQueryable()
                .QueryDynamic(null, "Location=1")
                .OfType<object>()
                .ToList();
            Assert.AreEqual(2, resultSet.Count);
            Assert.AreSame(users[1], resultSet[0]);
            Assert.AreSame(users[3], resultSet[1]);

            resultSet = users.AsQueryable()
                .QueryDynamic(null, "Name=Roy")
                .OfType<object>()
                .ToList();
            Assert.AreEqual(1, resultSet.Count);
            Assert.AreSame(users[2], resultSet[0]);
        }

        [Test]
        public void TestSelectorAndFilter()
        {
            var resultSet = users.AsQueryable()
                .QueryDynamic("Name,Age", "Age<30")
                .OfType<object>();
            Assert.AreEqual(
                @"[{""Age"":10,""Name"":""Alice""},{""Age"":18,""Name"":""Bob""},{""Age"":24,""Name"":""Roy""}]",
                JsonSerializer.Serialize(resultSet));

            resultSet = users.AsQueryable()
                .QueryDynamic("Name,Age", "Age in [18,30]")
                .OfType<object>();
            Assert.AreEqual(@"[{""Age"":18,""Name"":""Bob""},{""Age"":30,""Name"":""Jack""}]",
                JsonSerializer.Serialize(resultSet));

            resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location", "Location = 1")
                .OfType<object>();
            Assert.AreEqual(@"[{""Location"":1,""Name"":""Bob""},{""Location"":1,""Name"":""Jack""}]",
                JsonSerializer.Serialize(resultSet));

            resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location", "Location in [1]")
                .OfType<object>();
            Assert.AreEqual(@"[{""Location"":1,""Name"":""Bob""},{""Location"":1,""Name"":""Jack""}]",
                JsonSerializer.Serialize(resultSet));

            resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location", "Location in [1,2]")
                .OfType<object>();
            Assert.AreEqual(
                @"[{""Location"":2,""Name"":""Alice""},{""Location"":1,""Name"":""Bob""},{""Location"":2,""Name"":""Roy""},{""Location"":1,""Name"":""Jack""}]",
                JsonSerializer.Serialize(resultSet));

            resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location", "&(Location in [1])(Name=Jack)")
                .OfType<object>();
            Assert.AreEqual(
                @"[{""Location"":1,""Name"":""Jack""}]",
                JsonSerializer.Serialize(resultSet));

            resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location", "&(Location in [1])(Name=Roy)")
                .OfType<object>();
            Assert.AreEqual(@"[]", JsonSerializer.Serialize(resultSet));
        }

        [Test]
        public void TestSelectorAndFilterAndOrderBy()
        {
            var resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location,Age", "Location in [1,2]", "+Age,-Name")
                .OfType<object>();
            Assert.AreEqual(
                @"[{""Age"":10,""Location"":2,""Name"":""Alice""},{""Age"":18,""Location"":1,""Name"":""Bob""},{""Age"":24,""Location"":2,""Name"":""Roy""},{""Age"":30,""Location"":1,""Name"":""Jack""}]",
                JsonSerializer.Serialize(resultSet));

            users = new[]
            {
                new User {Name = "Alice", Age = 10, Location = Location.SanFrancisco},
                new User {Name = "Bob", Age = 10, Location = Location.NewYork},
                new User {Name = "Roy", Age = 10, Location = Location.SanFrancisco},
                new User {Name = "Jack", Age = 10, Location = Location.NewYork}
            };
            resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location,Age", "Location in [1,2]", "+Age,-Name")
                .OfType<object>();
            Assert.AreEqual(
                @"[{""Age"":10,""Location"":2,""Name"":""Roy""},{""Age"":10,""Location"":1,""Name"":""Jack""},{""Age"":10,""Location"":1,""Name"":""Bob""},{""Age"":10,""Location"":2,""Name"":""Alice""}]",
                JsonSerializer.Serialize(resultSet));
        }

        [Test]
        public void TestSelectorAndFilterAndOrderByAndSkip()
        {
            var resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location,Age", "Location in [1,2]", "+Age,-Name", 1)
                .OfType<object>();
            Assert.AreEqual(
                @"[{""Age"":18,""Location"":1,""Name"":""Bob""},{""Age"":24,""Location"":2,""Name"":""Roy""},{""Age"":30,""Location"":1,""Name"":""Jack""}]",
                JsonSerializer.Serialize(resultSet));

            users = new[]
            {
                new User {Name = "Alice", Age = 10, Location = Location.SanFrancisco},
                new User {Name = "Bob", Age = 10, Location = Location.NewYork},
                new User {Name = "Roy", Age = 10, Location = Location.SanFrancisco},
                new User {Name = "Jack", Age = 10, Location = Location.NewYork}
            };
            resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location,Age", "Location in [1,2]", "+Age,-Name", 4)
                .OfType<object>();
            Assert.AreEqual(
                @"[]",
                JsonSerializer.Serialize(resultSet));
        }


        [Test]
        public void TestSelectorAndFilterAndOrderByAndTake()
        {
            var resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location,Age", "Location in [1,2]", "+Age,-Name", null, 1)
                .OfType<object>();
            Assert.AreEqual(
                @"[{""Age"":10,""Location"":2,""Name"":""Alice""}]",
                JsonSerializer.Serialize(resultSet));

            users = new[]
            {
                new User {Name = "Alice", Age = 10, Location = Location.SanFrancisco},
                new User {Name = "Bob", Age = 10, Location = Location.NewYork},
                new User {Name = "Roy", Age = 10, Location = Location.SanFrancisco},
                new User {Name = "Jack", Age = 10, Location = Location.NewYork}
            };
            resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location,Age", "Location in [1,2]", "+Age,-Name", null, 4)
                .OfType<object>();
            Assert.AreEqual(
                @"[{""Age"":10,""Location"":2,""Name"":""Roy""},{""Age"":10,""Location"":1,""Name"":""Jack""},{""Age"":10,""Location"":1,""Name"":""Bob""},{""Age"":10,""Location"":2,""Name"":""Alice""}]",
                JsonSerializer.Serialize(resultSet));
        }

        [Test]
        public void TestSelectorAndFilterAndOrderByAndSkipAndTake()
        {
            var resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location,Age", "Location in [1,2]", "+Age,-Name", 1, 2)
                .OfType<object>();
            Assert.AreEqual(
                @"[{""Age"":18,""Location"":1,""Name"":""Bob""},{""Age"":24,""Location"":2,""Name"":""Roy""}]",
                JsonSerializer.Serialize(resultSet));

            users = new[]
            {
                new User {Name = "Alice", Age = 10, Location = Location.SanFrancisco},
                new User {Name = "Bob", Age = 10, Location = Location.NewYork},
                new User {Name = "Roy", Age = 10, Location = Location.SanFrancisco},
                new User {Name = "Jack", Age = 10, Location = Location.NewYork}
            };
            resultSet = users.AsQueryable()
                .QueryDynamic("Name,Location,Age", "Location in [1,2]", "+Age,-Name", 0, 0)
                .OfType<object>();
            Assert.AreEqual(
                @"[]",
                JsonSerializer.Serialize(resultSet));
        }
        [Test]
        public void TestUpdater()
        {
            // test the public api
            var user = new User { Name = "xxx" };
            var success = Utility.TryUpdateDynamic(user, "Age=18;Name=Roy;XXX=12;Location=2");
            Assert.IsTrue(success);
            Assert.AreEqual(18, user.Age);
            Assert.AreEqual("Roy", user.Name);
            Assert.AreEqual(Location.SanFrancisco, user.Location);

            // no compile needed
            success = Utility.TryUpdateDynamic(user, "Age=30;Name=Jack;XXX=asf;Location=1");
            Assert.IsTrue(success);
            Assert.AreEqual(30, user.Age);
            Assert.AreEqual("Jack", user.Name);
            Assert.AreEqual(Location.NewYork, user.Location);

            // compile needed, as update parameter changed
            success = Utility.TryUpdateDynamic(user, @"Age=30;Name=Jack\;XXX=asf;Location=1");
            Assert.IsTrue(success);
            Assert.AreEqual(30, user.Age);
            Assert.AreEqual(@"Jack;XXX=asf", user.Name);
            Assert.AreEqual(Location.NewYork, user.Location);

            // compile needed, as update parameter changed
            success = Utility.TryUpdateDynamic(user, @"Age=30;Name=Jack\;XXX=asf\;Location=2");
            Assert.IsTrue(success);
            Assert.AreEqual(30, user.Age);
            Assert.AreEqual(@"Jack;XXX=asf;Location=2", user.Name);
            Assert.AreEqual(Location.NewYork, user.Location);

            // compile needed, as update parameter changed
            success = Utility.TryUpdateDynamic(user, "Name=");
            Assert.IsTrue(success);
            Assert.AreEqual(30, user.Age);
            Assert.AreEqual("", user.Name);
            Assert.AreEqual(Location.NewYork, user.Location);

            success = Utility.TryUpdateDynamic(user, "BirthDay=");
            Assert.IsTrue(success);
            Assert.AreEqual(default(DateTime), user.BirthDay);

            success = Utility.TryUpdateDynamic(user, "BirthDay=1/1/2010 12:00:00 AM");
            Assert.IsTrue(success);
            Assert.AreEqual(new DateTime(2010, 1, 1), user.BirthDay);
        }
    }
}
