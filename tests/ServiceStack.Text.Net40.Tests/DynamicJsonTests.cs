#if NET40
using NUnit.Framework;

namespace ServiceStack.Text.Tests
{
    [TestFixture]
    public class DynamicJsonTests
    {
        [Test]
        public void Can_serialize_dynamic_instance()
        {
            var dog = new { Name = "Spot" };
            var json = DynamicJson.Serialize(dog);

            Assert.IsNotNull(json);
            json.Print();
        }

        [Test]
        public void Can_deserialize_dynamic_instance()
        {
            var dog = new { Name = "Spot" };
            var json = DynamicJson.Serialize(dog);
            var deserialized = DynamicJson.Deserialize(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(dog.Name, deserialized.Name);
        }
		
		[Test]
		public void Can_deserialize_dynamic_instance_and_parse_primitive_value_types()
        {
            var dog = new { Age = 5 };
            var json = DynamicJson.Serialize(dog);
            JsConfig.TryToParsePrimitiveTypeValues = true;
            var deserialized = DynamicJson.Deserialize(json);

            Assert.IsNotNull(deserialized);
            Assert.AreEqual(dog.Age, deserialized.Age);
        }
    }
}
#endif
