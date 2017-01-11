﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Newtonsoft.Json;

namespace GitHubBlog
{
    public partial class EditPage : ContentPage
    {
		bool IsNew; // 새로운 글을 작성하는 모드인가?

		public EditPage(bool isNew)
        {
            InitializeComponent();

			IsNew = isNew;
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
			if(IsNew)
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
			else
			{

			}
		}
	}

	static partial class Extension
	{
		public static string ToYaml(this Dictionary<string, string> dictionary)
		{
			StringBuilder builder = new StringBuilder();

			builder.AppendLine("---");
			foreach(var item in dictionary)
			{
				builder.AppendLine($"{item.Key}: {item.Value}");
			}
			builder.AppendLine("---");

			return builder.ToString();
			// TODO 하세용~
		}
	}
}
