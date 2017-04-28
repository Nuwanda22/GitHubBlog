using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GitHubBlog.Models
{
	class HttpResult
	{
		public bool IsSuccess { get; set; }
		public HttpStatusCode StatusCode { get; set; }
		public string Result { get; set; }
	}
}
