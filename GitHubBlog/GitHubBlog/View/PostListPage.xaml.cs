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
		ObservableCollection<Post> PostCollection;
		bool isFirst = true;

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
			
			if(CrossConnectivity.Current.IsConnected && isFirst)
			{
				StatusLabel.IsVisible = false;

				PostCollection.Clear();
				await Load(PostCollection);

				Indicator.IsVisible = false;
				Indicator.IsRunning = false;

				isFirst = false;
			}
			else if(!CrossConnectivity.Current.IsConnected)
			{
				Indicator.IsVisible = false;
				Indicator.IsRunning = false;
			}
		}

		private async Task<bool> Load(Collection<Post> collection)
		{
			// API를 통해 글 목록 정보가 담긴 json을 받음
			HttpResult result = await RestAPI.GetAsync("https://api.github.com/repos/Nuwanda22/nuwanda22.github.io/contents/_posts/");
			
			// 성공일 경우
			if(result.IsSuccess)
			{
				// json을 파싱하여 글 목록을 만들고
				JArray array = JArray.Parse(result.Result);
				foreach (var item in array.Reverse())
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
		
		private async void ToolbarItem_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(/*new EditPage(true)*/new PostEditPage());
		}
		
		private async void PostListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			(sender as ListView).SelectedItem = null;

			if (e.SelectedItem != null)
			{
				await Navigation.PushAsync(new PostPage(e.SelectedItem as Post));
			}
		}

		private async void MenuItem_Clicked(object sender, EventArgs e)
		{
			var item = sender as MenuItem;
			var param = item.CommandParameter as Post;
			
			if(item.Text == "Edit")
			{
				await Navigation.PushAsync(new EditPage(false));
			}
			else if(item.Text == "Delete")
			{
				if(await DisplayAlert("", "Do you really want to delete it?", "Yes", "No"))
				{
					var result = await RestAPI.DeleteAsync("https://api.github.com/repos/Nuwanda22/nuwanda22.github.io/contents/_posts/" + param.FileName,
					JsonConvert.SerializeObject(new
					{
						path = "_posts/" + param.FileName,
						message = "Deleted " + param.FileName,
						sha = param.SHA
					}), RestAPI.Key);

					if(result.IsSuccess)
					{
						await DisplayAlert("", "It has been deleted.", "OK");
						// TODO: Refresh PostListView
					}
					else
					{
						await DisplayAlert("An error has occurred.", "Please try again.", "OK");
					}
				}
			}
		}

		private async void PostListView_Refreshing(object sender, EventArgs e)
		{
			PostCollection.Clear();
			await Load(PostCollection);

			PostListView.IsRefreshing = false;
		}
	}

	public static partial class Extension
	{
		public static string ToFirtCharUpper(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new ArgumentNullException(nameof(input), "This string argument is null or empty.");
			}

			return input[0].ToString().ToUpper() + input.Substring(1);
		}
	}
}
