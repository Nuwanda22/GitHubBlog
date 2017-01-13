using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;

using Plugin.Connectivity;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitHubBlog
{
	public partial class PostListPage : ContentPage
	{
		bool isFirstTry = true;
		ObservableCollection<Post> PostCollection;
		string UserName;

		public PostListPage()
		{
			InitializeComponent();

			PostCollection = new ObservableCollection<Post>();
			PostListView.ItemsSource = PostCollection;

			CrossConnectivity.Current.ConnectivityChanged += (sender, e) =>
			{
				PostListView.IsVisible = e.IsConnected;
				StatusLabel.IsVisible = !e.IsConnected;
			};
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			// 사용자 이름을 구하여 타이틀에 표시
			UserName = await GetUserName();
			Title = (UserName + ".github.io").ToLower();

			// 인터넷에 연결되어 있고 첫번째라면
			if (CrossConnectivity.Current.IsConnected && isFirstTry)
			{
				// 인터넷 연결이 되지 않음 창을 숨기고
				StatusLabel.IsVisible = false;

				// 글 목록 초기화 후 받아온다.
				PostCollection.Clear(); ;
				await PostCollection.Load(UserName, UserName + ".github.io");

				// 글 목록을 다 받아오면 로딩 창을 숨긴다.
				Indicator.IsVisible = false;
				Indicator.IsRunning = false;

				// 이제 첫번째 아님
				isFirstTry = false;
			}
			// 인터넷에 연결되지 않았다면
			else if (!CrossConnectivity.Current.IsConnected)
			{
				// 로딩 창을 숨긴다.
				Indicator.IsVisible = false;
				Indicator.IsRunning = false;
			}
		}

		private async Task<string> GetUserName()
		{
			// 만약 username이 저장되지 않았다면
			if (!App.Current.Properties.ContainsKey("username"))
			{
				// 토큰을 통해 사용자 이름 요청 후
				var usernameResult = await RestAPI.GetAsync($"https://api.github.com/user", token: App.Current.Properties["token"] as string);
				if (usernameResult.IsSuccess)
				{
					// 성공 시 사용자 이름 저장 및 리턴
					string username = JObject.Parse(usernameResult.Result)["login"].Value<string>();
					App.Current.Properties.Add("username", username);
					return username;
				}
				else { return null; }
			}
			else
			{
				// 저장되어 있는 경우 바로 리턴
				return App.Current.Properties["username"] as string;
			}
		}

		private async void PostToolbarItem_Clicked(object sender, EventArgs e)
		{
			// 글 작성 페이지를 띄움
			await Navigation.PushAsync(new PostEditPage(true));
		}

		private async void PostListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			// 선택 없애기
			(sender as ListView).SelectedItem = null;

			// 글이 선택되었다면 글 페이지를 띄움
			if (e.SelectedItem != null)
			{
				await Navigation.PushAsync(new PostPage(e.SelectedItem as Post));
			}
		}

		private async void MenuItem_Clicked(object sender, EventArgs e)
		{
			// 버튼과 글 객체를 받음
			var item = sender as MenuItem;
			var param = item.CommandParameter as Post;

			// 수정 버튼이면
			if (item.Text == "Edit")
			{
				// 수정 페이지를 띄운다.
				await Navigation.PushAsync(new PostEditPage(false));
			}
			// 삭제 버튼이면
			else if (item.Text == "Delete")
			{
				// 삭제 여부 확인 후 삭제를 원하면
				if (await DisplayAlert("", "Do you really want to delete it?", "Yes", "No"))
				{
					// 삭제 요청
					var result = await RestAPI.DeleteAsync($"https://api.github.com/repos/{UserName}/{UserName}.github.io/contents/_posts/" + param.FileName,
					JsonConvert.SerializeObject(new
					{
						path = "_posts/" + param.FileName,
						message = "Deleted " + param.FileName,
						sha = param.SHA
					}), App.Current.Properties["token"] as string);

					// 요청 성공 시 삭제됨을 표시
					if (result.IsSuccess)
					{
						await DisplayAlert("", "It has been deleted.", "OK");
						// TODO: Refresh PostListView
					}
					// 실패 시 실패함을 표시
					else
					{
						await DisplayAlert("An error has occurred.", "Please try again.", "OK");
					}
				}
			}
		}

		private async void PostListView_Refreshing(object sender, EventArgs e)
		{
			// 새로 고침시 글 목록 초기화 후 다시 불러옴
			PostCollection.Clear();
			await PostCollection.Load(UserName, UserName + ".github.io");

			// 새로 고침 종료
			PostListView.IsRefreshing = false;
		}

		private async void ToolbarItem_Clicked(object sender, EventArgs e)
		{
			// 로그아웃 여부를 다시 확인하고
			if(await DisplayAlert("", "Are you sure you want to sign out?", "OK", "Cancel"))
			{
				// 원하는 경우 저장된 토큰 및 사용자 이름 삭제 후
				App.Current.Properties.Remove("token");
				App.Current.Properties.Remove("username");

				// 로그인 페이지를 띄움
				App.Current.MainPage = new LoginPage(true);
			}
		}
	}

	static partial class Extension
	{
		public static string ToFirtCharUpper(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new ArgumentNullException(nameof(input), "This string argument is null or empty.");
			}

			return input[0].ToString().ToUpper() + input.Substring(1);
		}

		public static async Task<bool> Load(this Collection<Post> collection, string user, string repo)
		{
			// API를 통해 글 목록 정보가 담긴 json을 받음
			HttpResult result = await RestAPI.GetAsync($"https://api.github.com/repos/{user}/{repo}/contents/_posts/");

			// 성공일 경우
			if (result.IsSuccess)
			{
				// json을 파싱하여 글 목록을 만들고 (날짜 역순)
				foreach (var item in JArray.Parse(result.Result).Reverse())
				{
					// 글 하나의 파일 이름 추출 후
					string fileName = item["name"].Value<string>();

					// 글 객체를 생성하여
					var newPost = new Post
					{
						FileName = fileName,
						Date = DateTime.Parse(fileName.Substring(0, 10)),
						Title = fileName.Substring(11).Replace('-', ' ').Split('.')[0].ToFirtCharUpper(),
						SHA = item["sha"].Value<string>()
					};

					// 리스트에 추가한다.
					collection.Add(newPost);
				}
			}

			// API의 성공 여부를 리턴함
			return result.IsSuccess;
		}
	}
}
