using System;
using NUnit.Framework;

namespace RoyLab.QData.Updater
{
    public class TestUpdaterBuilder
    {
        [Test]
        public void TestUpdaterBuilderBasicCase()
        {
            var user = new User {Age = 18, Name = "Roy"};

            var parseSuccess = UpdaterParser.TryParse("Age=99;Name=xxx", out var parsedExpressions, out var valueArray);
            Assert.IsTrue(parseSuccess);
            var dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(user, "24", "roy\\");
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(DateTime.MinValue, user.BirthDay);
            Assert.AreEqual(Guid.Empty, user.ID);
            Assert.AreEqual(0, (int) user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"Age=;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(0, user.Age);
            Assert.AreEqual(DateTime.MinValue, user.BirthDay);
            Assert.AreEqual(Guid.Empty, user.ID);
            Assert.AreEqual(0, (int) user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"Age=invalid_value;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(0, user.Age);
            Assert.AreEqual(DateTime.MinValue, user.BirthDay);
            Assert.AreEqual(Guid.Empty, user.ID);
            Assert.AreEqual(0, (int) user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"Age=24;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(DateTime.MinValue, user.BirthDay);
            Assert.AreEqual(Guid.Empty, user.ID);
            Assert.AreEqual(0, (int) user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"BirthDay=;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(DateTime.MinValue, user.BirthDay);
            Assert.AreEqual(Guid.Empty, user.ID);
            Assert.AreEqual(0, (int) user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"BirthDay=invalid_value;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(DateTime.MinValue, user.BirthDay);
            Assert.AreEqual(Guid.Empty, user.ID);
            Assert.AreEqual(0, (int) user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"BirthDay=5/22/2015 12:00:00 AM;", out parsedExpressions,
                out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.BirthDay);
            Assert.AreEqual(Guid.Empty, user.ID);
            Assert.AreEqual(0, (int) user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"ID=;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.BirthDay);
            Assert.AreEqual(Guid.Empty, user.ID);
            Assert.AreEqual(0, (int) user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"ID=invalid_id;", out parsedExpressions,
                out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.BirthDay);
            Assert.AreEqual(Guid.Empty, user.ID);
            Assert.AreEqual(0, (int) user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"ID=db4f8676-08ae-46bd-a058-2a57864274cd;", out parsedExpressions,
                out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.BirthDay);
            Assert.AreEqual(Guid.Parse("db4f8676-08ae-46bd-a058-2a57864274cd"), user.ID);
            Assert.AreEqual(0, (int) user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"Location=invalid;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.BirthDay);
            Assert.AreEqual(Guid.Parse("db4f8676-08ae-46bd-a058-2a57864274cd"), user.ID);
            Assert.AreEqual(0, (int) user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"Location=1;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.BirthDay);
            Assert.AreEqual(Guid.Parse("db4f8676-08ae-46bd-a058-2a57864274cd"), user.ID);
            Assert.AreEqual(1, (int) user.Location);
            Assert.AreEqual(Location.NewYork, user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"Location=10;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.BirthDay);
            Assert.AreEqual(Guid.Parse("db4f8676-08ae-46bd-a058-2a57864274cd"), user.ID);
            Assert.AreEqual(10, (int) user.Location);
            Assert.AreEqual(@"roy\", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"Name=;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.BirthDay);
            Assert.AreEqual(Guid.Parse("db4f8676-08ae-46bd-a058-2a57864274cd"), user.ID);
            Assert.AreEqual(10, (int) user.Location);
            Assert.AreEqual("", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"Name=roy\\\;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.BirthDay);
            Assert.AreEqual(Guid.Parse("db4f8676-08ae-46bd-a058-2a57864274cd"), user.ID);
            Assert.AreEqual(10, (int) user.Location);
            Assert.AreEqual(@"roy\;", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"ParentalDay=;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.BirthDay);
            Assert.AreEqual(Guid.Parse("db4f8676-08ae-46bd-a058-2a57864274cd"), user.ID);
            Assert.AreEqual(10, (int) user.Location);
            Assert.AreEqual(@"roy\;", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"ParentalDay=invalid_value;", out parsedExpressions, out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.BirthDay);
            Assert.AreEqual(Guid.Parse("db4f8676-08ae-46bd-a058-2a57864274cd"), user.ID);
            Assert.AreEqual(10, (int) user.Location);
            Assert.AreEqual(@"roy\;", user.Name);
            Assert.AreEqual(null, user.ParentalDay);

            parseSuccess = UpdaterParser.TryParse(@"ParentalDay=5/22/2015 12:00:00 AM;", out parsedExpressions,
                out valueArray);
            Assert.IsTrue(parseSuccess);
            dynamicFunction = UpdaterBuilder.Build(parsedExpressions, typeof(User)).Compile();
            valueArray[0] = user;
            dynamicFunction.DynamicInvoke(valueArray);
            Assert.AreEqual(24, user.Age);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.BirthDay);
            Assert.AreEqual(Guid.Parse("db4f8676-08ae-46bd-a058-2a57864274cd"), user.ID);
            Assert.AreEqual(10, (int) user.Location);
            Assert.AreEqual(@"roy\;", user.Name);
            Assert.AreEqual(new DateTime(2015, 5, 22), user.ParentalDay);
        }
    }
}