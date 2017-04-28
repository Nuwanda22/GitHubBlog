using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Newtonsoft.Json.Linq;

using GitHubBlog.Models;
using GitHubBlog.Libraries;

namespace GitHubBlog.Pages
{
	public partial class PostPage : ContentPage
	{
		Post Post;

		public PostPage(Post post)
		{
			InitializeComponent();

			Post = post;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			Indicator.IsRunning = true;
			
			var result = await RestAPI.GetAsync("https://api.github.com/repos/Nuwanda22/nuwanda22.github.io/contents/_posts/" + Post.FileName);
			if (result.IsSuccess)
			{
				JObject json = JObject.Parse(result.Result);

				var encode = Convert.FromBase64String(json["content"].Value<string>());
				string decode = Encoding.UTF8.GetString(encode, 0, encode.Length);

				PostWebView.Source = new HtmlWebViewSource { Html = CommonMark.CommonMarkConverter.Convert(Regex.Split(decode, "---")[2]) };
			}

			Indicator.IsRunning = false;
		}
	}
}
