using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using YamlDotNet.Serialization;

namespace GitHubBlog.Libraries
{
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
