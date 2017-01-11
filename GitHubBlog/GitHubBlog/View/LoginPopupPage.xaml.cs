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
		public LoginPopupPage()
		{
			InitializeComponent();
		}

		private async void OnCloseButtonTapped(object sender, EventArgs e)
		{
			await Navigation.PopAllPopupAsync();
		}

		protected override bool OnBackgroundClicked()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				await Navigation.PopAllPopupAsync();
			});

			return false;
		}

		private async void LoginWebView_Navigating(object sender, WebNavigatingEventArgs e)
		{
			// 페이지 이동 전 url을 받아와 로그인 결과 창으로 가는지 알아낸 후
			string url = e.Url;
			if (url.Contains("https://nuwanda22.github.io/callback"))
			{
				// 로그인 결과 창으로 갈 예정이었다면 결과 값들을 추출
				string result = url.Substring(url.IndexOf('?') + 1);

				// 결과가 성공이었다면
				if (result.Contains("code"))
				{
					// 현재 팝업을 닫고 (WebView가 인터넷을 독점하고 있기 때문에 먼저 종료)
					await Navigation.PopAllPopupAsync();
					var loginPage = Navigation.NavigationStack.First() as LoginPage;

					// OAuth token을 얻기 위한 code 추출
					string code = result.Substring(result.IndexOf('=') + 1);

					// OAuth token 요청
					var result2 = await RestAPI.GetAsync(
						$"https://github.com/login/oauth/access_token?client_id=3af3751f46683292dc37&client_secret=f409b10d0b6bdda3a37b47bada5b7645e078f421&code={code}",
						new Dictionary<string, string> { { "Accept", "application/json" } });
					// 성공했다면
					if (result2.IsSuccess)
					{
						// 키를 저장하고 메인 페이지를 뛰움
						App.Current.Properties.Add("token", JObject.Parse(result2.Result)["access_token"].Value<string>());
						App.Current.MainPage = new NavigationPage(new PostListPage());
					}
					// 실패했다면
					else
					{
						// 실패 코드 출력 및 팝업 닫기
						await DisplayAlert("An error has occurred", result2.Result, "OK");
						await Navigation.PopAllPopupAsync();
					}
				}
				// 실패였다면
				else
				{
					// 실패 코드 출력 및 팝업 닫기
					await DisplayAlert("An error has occurred", result, "OK");
					await Navigation.PopAllPopupAsync();
				}
			}
		}
	}
}
