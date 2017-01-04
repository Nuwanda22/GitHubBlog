using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubBlog
{
	class APIContent
	{
		private string Title;
		private string Markdown;

		public DateTime Date { get; set; }
		public string Desc { get; set; }
		public string[] Keywords { get; set; }
		public string[] Categories { get; set; }
		public string[] Tags { get; set; }
		public string Icon { get; set; }

		public APIContent(string title, string markDown)
		{
			Title = title;
			Markdown = markDown;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			builder.AppendLine("---");
			builder.AppendLine("layout: post");
			builder.AppendLine($"title: \"{Title}\"");
			if (Date == default(DateTime)) Date = DateTime.Now;
			builder.AppendLine($"date: {Date.ToString("yyyy-MM-dd")}");
			builder.AppendLine($"desc: \"{Desc}\"");
			builder.AppendLine($"keywords: \"{Keywords.ToEnumeratedString()}\"");
			builder.AppendLine($"categories: [{Categories.ToEnumeratedString()}]");
			builder.AppendLine($"tags: [{Tags.ToEnumeratedString()}]");
			builder.AppendLine($"icon: {Icon}");
			builder.AppendLine("---");
			builder.AppendLine();

			builder.AppendLine(Markdown);

			return Convert.ToBase64String(Encoding.UTF8.GetBytes(builder.ToString()));
		}
	}

	static partial class Extension
	{
		public static string ToEnumeratedString(this string[] args)
		{
			StringBuilder builder = new StringBuilder();

			int i = 1;
			foreach (var s in args)
			{
				builder.Append(s + (args.Length != i++ ? "," : ""));
			}

			return builder.ToString();
		}
	}
}
