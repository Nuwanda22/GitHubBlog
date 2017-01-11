using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitHubBlog
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

				var result2 = await RestAPI.PostAsync("https://api.github.com/markdown", JsonConvert.SerializeObject(new
				{
					text = Regex.Split(decode, "---")[2]
				}), App.Current.Properties["token"] as string);

				if (result2.IsSuccess)
				{
					PostWebView.Source = new HtmlWebViewSource { Html = result2.Result };
				}
			}

			Indicator.IsRunning = false;
		}
	}
}
