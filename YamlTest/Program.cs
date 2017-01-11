using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Newtonsoft.Json;

using YamlDotNet.Serialization;
using YamlDotNet.RepresentationModel;

namespace YamlTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var obj = new
			{
				layout = "post",
				title = new string[] { "제목", "asdasd" }
			};
			string yamlString = new Serializer().Serialize(obj);


			var r = new StringReader(yamlString);
			var deserializer = new Deserializer();
			var yamlObject = deserializer.Deserialize(r) as Dictionary<object, object>;

			var sequence = yamlObject["layout"];

			Console.WriteLine(sequence);
		}
	}

	static class Yaml
	{
		public static string SerializeObject(object value)
		{
			return new Serializer().Serialize(value);
		}

		public static Dictionary<object, object> DeserializeObject(string yaml)
		{
			Dictionary<object, object> temp = null;

			using (var reader = new StringReader(yaml))
			{
				var deserializer = new Deserializer();

				temp = deserializer.Deserialize(reader) as Dictionary<object, object>;
			}

			return temp;
		}
	}
}
