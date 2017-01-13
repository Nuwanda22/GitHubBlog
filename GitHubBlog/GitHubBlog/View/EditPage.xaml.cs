using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Newtonsoft.Json;

using Rg.Plugins.Popup.Extensions;

namespace GitHubBlog
{
	public partial class EditPage : ContentPage
	{
		bool IsNew; // 새로운 글을 작성하는 모드인가?
		Dictionary<object, object> YamlDictionary;

		public EditPage(bool isNew)
		{
			InitializeComponent();
			
			IsNew = isNew;
			YamlDictionary = new Dictionary<object, object> { { "layout","post" }, { "title", "" } };
		}

		public string EditorText
		{
			get
			{
				return ContentEditor.Text;
			}
		}

		private async void SendToolbarItem_Clicked(object sender, EventArgs e)
		{
			// 새로운 글이면
			if (IsNew)
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
				var result = await RestAPI.PutAsync(url, json, App.Current.Properties["token"] as string);
				await DisplayAlert("", result.Result, "OK");
			}
			else
			{

			}
		}

		private async void ToolbarItem_Clicked(object sender, EventArgs e)
		{
			YamlDictionary["title"] = TitleEntry.Text;
			await Navigation.PushPopupAsync(new YamlEditPopupPage(YamlDictionary));
		}
	}
}
