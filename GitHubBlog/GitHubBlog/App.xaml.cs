using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using Newtonsoft.Json.Linq;

namespace GitHubBlog
{
	public partial class App : Application
	{
		static App current;

		public App()
		{
			InitializeComponent();

			current = this;

			// 이전에 로그인했다면 
			if (Properties.ContainsKey("token"))
			{
				// 만약 username이 초기화되지 않았다면 초기화 한 후에
				if(!Properties.ContainsKey("username"))
				{
					Device.BeginInvokeOnMainThread(async () => 
					{
						// 토큰을 통해 사용자 이름 요청
						var usernameResult = await RestAPI.GetAsync($"https://api.github.com/user", token: Properties["token"] as string);
						if (usernameResult.IsSuccess)
						{
							// 성공 시 사용자 이름 저장 후
							string username = JObject.Parse(usernameResult.Result)["login"].Value<string>();
							Properties.Add("username", username);
						}
					});
				}

				// 메인 페이지를 띄운다.
				MainPage = new NavigationPage(new PostListPage());
			}
			// 그게 아니라면 로그인 페이지를 띄운다.
			else
			{
				Properties.Remove("token");
				MainPage = new LoginPage();
			}
		}

		public static new App Current
		{
			get
			{
				return current;
			}
		}
	}
}
