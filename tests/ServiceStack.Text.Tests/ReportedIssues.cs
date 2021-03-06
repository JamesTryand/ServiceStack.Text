using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ServiceStack.ServiceInterface.ServiceModel;

namespace ServiceStack.Text.Tests
{
	[TestFixture]
	public class ReportedIssues
		: TestBase
	{

		[Test]
		public void Issue5_Can_serialize_Dictionary_with_null_value()
		{
			var map = new Dictionary<string, string> {
				{"p1","v1"},{"p2","v2"},{"p3",null},
			};

			Serialize(map);
		}

		public abstract class CorrelativeDataBase
		{
			protected CorrelativeDataBase()
			{
				CorrelationIdentifier = GetNextId();
			}

			public Guid CorrelationIdentifier { get; set; }

			protected static Guid GetNextId()
			{
				return Guid.NewGuid();
			}
		}

		public sealed class TestObject : CorrelativeDataBase
		{
			public Type SomeType { get; set; }
			public string SomeString { get; set; }
			public IEnumerable<Type> SomeTypeList { get; set; }
			public IEnumerable<Type> SomeTypeList2 { get; set; }
			public IEnumerable<object> SomeObjectList { get; set; }
		}

		[Test]
		public void Serialize_object_with_type_field()
		{
			var obj = new TestObject
			{
				SomeType = typeof(string),
				SomeString = "Test",
				SomeObjectList = new object[0]
			};

			Serialize(obj, includeXml: false); // xml cannot serialize Type objects.
		}

		[Test]
		public void Serialize_object_with_type_field2()
		{

			var obj = new TestObject
			{
				SomeType = typeof(string),
				SomeString = "Test",
				SomeObjectList = new object[0]
			};

			var strModel = TypeSerializer.SerializeToString<object>(obj);
			Console.WriteLine("Len: " + strModel.Length + ", " + strModel);
			var toModel = TypeSerializer.DeserializeFromString<TestObject>(strModel);
		}

		public class Article
		{
			public string title { get; set; }
			public string url { get; set; }
			public string author { get; set; }
			public string author_id { get; set; }
			public string date { get; set; }
			public string type { get; set; }
		}

		[Test]
		public void Serialize_Dictionary_with_backslash_as_last_char()
		{
			var map = new Dictionary<string, Article>
          	{
				{
					"http://www.eurogamer.net/articles/2010-09-14-vanquish-limited-edition-has-7-figurine",
					new Article
					{
						title = "Vanquish Limited Edition has 7\" figurine",
						url = "articles/2010-09-14-vanquish-limited-edition-has-7-figurine",
						author = "Wesley Yin-Poole",
						author_id = "621",
						date = "14/09/2010",
						type = "news",
					}
				},
				{
					"http://www.eurogamer.net/articles/2010-09-14-supercar-challenge-devs-next-detailed",
					new Article
					{
						title = "SuperCar Challenge dev's next detailed",
						url = "articles/2010-09-14-supercar-challenge-devs-next-detailed",
						author = "Wesley Yin-Poole",
						author_id = "621",
						date = "14/09/2010",
						type = "news",
					}
				},
				{
					"http://www.eurogamer.net/articles/2010-09-14-hmv-to-sell-dead-rising-2-a-day-early",
					new Article
					{
						title = "HMV to sell Dead Rising 2 a day early",
						url = "articles/2010-09-14-hmv-to-sell-dead-rising-2-a-day-early",
						author = "Wesley Yin-Poole",
						author_id = "621",
						date = "14/09/2010",
						type = "News",
					}
				},
          	};

			Serialize(map);

			var json = JsonSerializer.SerializeToString(map);
			var fromJson = JsonSerializer.DeserializeFromString<Dictionary<string, Article>>(json);

			Assert.That(fromJson, Has.Count.EqualTo(map.Count));
		}

		public class Item
		{
			public int type { get; set; }
			public int color { get; set; }
		}
		public class Basket
		{
			public Basket()
			{
				Items = new Dictionary<Item, int>();
			}
			public Dictionary<Item, int> Items { get; set; }
		}

		[Test]
		public void Can_Serialize_Class_with_Typed_Dictionary()
		{
			var basket = new Basket();
			basket.Items.Add(new Item { type = 1, color = 2 }, 10);
			basket.Items.Add(new Item { type = 4, color = 1 }, 20);

			Serialize(basket);
		}

        public class Book
        {
            public UInt64 Id { get; set; }
            public UInt64 OwnerUserId { get; set; }
            public String Title { get; set; }
            public String Description { get; set; }
            public UInt16 CategoryId { get; set; }
            public DateTime CreatedDateTime { get; set; }
            public Byte Icon { get; set; }
            public Boolean Canceled { get; set; }
        }

        public class BookResponse : IHasResponseStatus
        {
            public Book Book { get; set; }
            public ResponseStatus ResponseStatus { get; set; }
        }

        public object GetBook()
        {
            var response = new BookResponse();
            return response;
        }

        [Test]
        public void Does_not_serialize_typeinfo_for_concrete_types()
        {
            var json = GetBook().ToJson();
            Console.WriteLine(json);
            Assert.That(json.IndexOf("__"), Is.EqualTo(-1));

        	var jsv = GetBook().ToJsv();
			Assert.That(jsv.IndexOf("__"), Is.EqualTo(-1));
		}

		public class TextTags
		{
			public string Text { get; set; }
			public string[] Tags { get; set; }
		}

		[Test]
		public void Can_serialize_sweedish_chars()
		{
			var dto = new TextTags { Text = "Olle �r en �L �l", Tags = new[] { "�l", "�l", "m�l" } };
			Serialize(dto);
		}

        [Test]
        public void Objects_Do_Not_Survive_RoundTrips_Via_StringStringDictionary_Due_To_DoubleQuoted_Properties()
        {
            var book = new Book();
            book.Id = 1234;
            book.Title = "ServiceStack in Action";
            book.CategoryId = 16;
            book.Description = "Manning eBooks";


            var json = book.ToJson();
            Console.WriteLine("Book to Json: " + json);

            var dictionary = json.FromJson<Dictionary<string, string>>();
            Console.WriteLine("Json to Dictionary: " + dictionary.Dump());

            var fromDictionary = dictionary.ToJson();
            Console.WriteLine("Json from Dictionary: " + fromDictionary);

			var fromJsonViaDictionary = fromDictionary.FromJson<Book>();

            Assert.AreEqual(book.Description, fromJsonViaDictionary.Description);
            Assert.AreEqual(book.Id, fromJsonViaDictionary.Id);
            Assert.AreEqual(book.Title, fromJsonViaDictionary.Title);
            Assert.AreEqual(book.CategoryId, fromJsonViaDictionary.CategoryId);
        }

		public class Test
		{
			public IDictionary<string, string> Items { get; set; }
			public string TestString { get; set; }
		}

		[Test]
		public void Does_Trailing_Backslashes()
		{
			var test = new Test {
				TestString = "Test",
				Items = new Dictionary<string, string> { { "foo", "bar\\" } }
			};

			var serialized = JsonSerializer.SerializeToString(test);
			Console.WriteLine(serialized);
			var deserialized = JsonSerializer.DeserializeFromString<Test>(serialized);

			Assert.That(deserialized.TestString, Is.EqualTo("Test")); // deserialized.TestString is NULL
		}
	}
}