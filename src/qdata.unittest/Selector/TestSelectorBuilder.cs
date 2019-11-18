using System.Text.Json;
using NUnit.Framework;

namespace RoyLab.QData.Selector
{
    public class TestSelectorBuilder
    {
        private User singleUser;

        [SetUp]
        public void InitializeTestData()
        {
            singleUser = new User {Name = "Roy", Age = 30, Location = Location.SanFrancisco};
        }

        [Test]
        public void TestSelector()
        {
            var selectorFunction = SelectorBuilder
                .Build(typeof(User), "Name,Age", out var outputType)
                ?.Compile();
            Assert.IsNotNull(selectorFunction);
            Assert.IsNotNull(outputType);
            var selectedObject = selectorFunction.DynamicInvoke(singleUser);
            Assert.IsInstanceOf(outputType, selectedObject);
            Assert.AreEqual("{\"Age\":30,\"Name\":\"Roy\"}", JsonSerializer.Serialize(selectedObject));

            selectorFunction = SelectorBuilder
                .Build(typeof(User), "Name, , ,,, ", out outputType)
                ?.Compile();
            Assert.IsNotNull(selectorFunction);
            Assert.IsNotNull(outputType);
            selectedObject = selectorFunction.DynamicInvoke(singleUser);
            Assert.IsInstanceOf(outputType, selectedObject);
            Assert.AreEqual("{\"Name\":\"Roy\"}", JsonSerializer.Serialize(selectedObject));

            selectorFunction = SelectorBuilder
                .Build(typeof(User), "Name,Name,Name", out outputType)
                ?.Compile();
            Assert.IsNotNull(selectorFunction);
            Assert.IsNotNull(outputType);
            selectedObject = selectorFunction.DynamicInvoke(singleUser);
            Assert.IsInstanceOf(outputType, selectedObject);
            Assert.AreEqual("{\"Name\":\"Roy\"}", JsonSerializer.Serialize(selectedObject));

            selectorFunction = SelectorBuilder
                .Build(typeof(User), "xx", out outputType)
                ?.Compile();
            Assert.IsNull(selectorFunction);
            Assert.IsNull(outputType);

            selectorFunction = SelectorBuilder
                .Build(typeof(User), " ", out outputType)
                ?.Compile();
            Assert.IsNull(selectorFunction);
            Assert.IsNull(outputType);

            selectorFunction = SelectorBuilder
                .Build(typeof(User), null, out outputType)
                ?.Compile();
            Assert.IsNull(selectorFunction);
            Assert.IsNull(outputType);
        }
    }
}