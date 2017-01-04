using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Newtonsoft.Json;

namespace GitHubBlog
{
    public partial class PostingPage : ContentPage
    {
        public PostingPage()
        {
            InitializeComponent();
		}

		private async void SendButton_Clicked(object sender, EventArgs e)
        {
			// 제목 및 글 준비
			// Hello World -> 2017-01-01-hello-world.md
			string fileName = DateTime.Now.Date.ToString("yyyy-MM-dd-") + SubTitleEntry.Text.ToLower().Replace(' ', '-') + ".md";
			string content = new APIContent(TitleEntry.Text, ContentEditor.Text)
			{
				Date = DateTime.Now,
				Desc = "",
				Keywords = new string[] { },
				Categories = new string[] { "Etc" },
				Tags = new string[] { },
				Icon = "fa-globe"
			}.ToString();

			// API 요청 준비
			string url = $"https://api.github.com/repos/Nuwanda22/nuwanda22.github.io/contents/_posts/{fileName}";
			string json = JsonConvert.SerializeObject(new
			{
				path = $"_posts/{fileName}",
				message = $"{SubTitleEntry.Text} Posted",
				content = content
			});

			// 요청 후 결과 출력
			var result = await RestAPI.PutAsync(url, json, RestAPI.Key);
			await DisplayAlert("", result.Result, "OK");
		}

		private async void ContentEditor_TextChanged(object sender, TextChangedEventArgs e)
		{
			// 바뀐 글을 가져와 API를 요청함
			var result = await RestAPI.PostAsync("https://api.github.com/markdown", JsonConvert.SerializeObject(new
			{
				text = e.NewTextValue
			}), RestAPI.Key);

			if(result.IsSuccess)
			{
				Previewer.Source = new HtmlWebViewSource { Html = result.Result };
			}
		}

		private async void ShowButton_Clicked(object sender, EventArgs e)
		{
			//var result = await RestAPI.GetAsync("https://api.github.com/repos/Nuwanda22/nuwanda22.github.io/contents/_posts/");

			//JArray arr = JArray.Parse(result.Result);
			//List<string> list = new List<string>();

			//foreach (var item in arr)
			//{
			//	list.Add(item["name"].Value<string>());
			//}

			await Navigation.PushAsync(new PostListPage());
		}
	}
}
