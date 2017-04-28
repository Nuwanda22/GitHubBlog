using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubBlog.Models
{
	public class Post
	{
		public string FileName { get; set; }
		public DateTime Date { get; set; }
		public string Title { get; set; }
		public string SHA { get; set; }
	}
}
