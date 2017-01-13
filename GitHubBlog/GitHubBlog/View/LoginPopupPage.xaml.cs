using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Extensions;

using Newtonsoft.Json.Linq;

namespace GitHubBlog
{
	public partial class LoginPopupPage : PopupPage
	{
		public LoginPopupPage(bool isAfterLogout)
		{
			InitializeComponent();
			
			if(isAfterLogout)
			{
				LoginWebView.Source = new Uri("https://github.com/logout");
			}
		}

		protected override bool OnBackgroundClicked()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				await Navigation.PopAllPopupAsync();
			});

			return false;
		}

		private async void OnCloseButtonTapped(object sender, EventArgs e)
		{
			await Navigation.PopAllPopupAsync();
		}

		private async void LoginWebView_Navigating(object sender, WebNavigatingEventArgs e)
		{
			// 페이지 이동 전 url을 받아온다
			string url = e.Url;
			// 로그아웃 후 메인페이지로 이동한다면
			if(url == "https://github.com/")
			{
				// 인증 창으로 이동시킨다.
				LoginWebView.Source = new Uri("https://github.com/login/oauth/authorize?client_id=3af3751f46683292dc37&scope=repo");
			}
			// 로그인 결과 창으로 간다면
			else if (url.Contains("https://nuwanda22.github.io/callback"))
			{
				// 로그인 결과 창으로 갈 예정이었다면 결과 값들을 추출
				string loginResult = url.Substring(url.IndexOf('?') + 1);

				// 결과가 성공이었다면
				if (loginResult.Contains("code"))
				{
					// 현재 팝업을 닫고 로그인 창에 로딩 창을 띄움
					await Navigation.PopAllPopupAsync();
					(App.Current.MainPage as LoginPage).Loading(true);

					// OAuth token을 얻기 위한 code 추출
					string code = loginResult.Substring(loginResult.IndexOf('=') + 1);

					// OAuth token 요청
					var tokenResult = await RestAPI.GetAsync(
						$"https://github.com/login/oauth/access_token?client_id=3af3751f46683292dc37&client_secret=f409b10d0b6bdda3a37b47bada5b7645e078f421&code={code}",
						new Dictionary<string, string> { { "Accept", "application/json" } });
					if (tokenResult.IsSuccess)
					{
						// 성공 시 토큰 저장
						string key = JObject.Parse(tokenResult.Result)["access_token"].Value<string>();
						App.Current.Properties.Add("token", key);

						// 사용자 이름을 모르는 경우
						if(!App.Current.Properties.ContainsKey("username"))
						{
							// 토큰을 통해 사용자 이름 요청
							var usernameResult = await RestAPI.GetAsync($"https://api.github.com/user", token: key);
							if (usernameResult.IsSuccess)
							{
								// 성공 시 사용자 이름 저장 후
								string username = JObject.Parse(usernameResult.Result)["login"].Value<string>();
								App.Current.Properties.Add("username", username);

								// 글 목록 페이지 띄움
								App.Current.MainPage = new NavigationPage(new PostListPage());
							}
							else
							{
								// 유저 이름을 가져오는데 실패했을 경우 로그만 남긴다.
								// 추후에 PostEditPage에서 로딩한다.
								System.Diagnostics.Debug.WriteLine(usernameResult.StatusCode + " : " + usernameResult.Result);
							}
						}
						else
						{
							// 글 목록 페이지 띄움
							App.Current.MainPage = new NavigationPage(new PostListPage());
						}
					}
					else
					{
						// 토큰을 가져오지 못한 경우 에러 메시지를 띄우고 로그를 남긴다.
						(App.Current.MainPage as LoginPage).Loading(false);
						System.Diagnostics.Debug.WriteLine(tokenResult.StatusCode + " : " + tokenResult.Result);
						await DisplayAlert("An error has occurred", "Please try again.", "OK");
					}
				}
				else
				{
					System.Diagnostics.Debug.WriteLine(loginResult);
					await DisplayAlert("An error has occurred", "Please try again.", "OK");
				}
			}
		}
	}
}
