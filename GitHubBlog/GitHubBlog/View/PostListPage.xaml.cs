using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Xamarin.Forms;

using Newtonsoft.Json.Linq;

namespace GitHubBlog
{
	public partial class PostListPage : ContentPage
	{
		ObservableCollection<Post> PostList;
		bool isFirst = true;

		public PostListPage()
		{
			InitializeComponent();

			PostList = new ObservableCollection<Post>();
			PostListView.ItemsSource = PostList;
		}
		
		private async Task<bool> Load(Collection<Post> list)
		{
			// API를 통해 글 목록 정보가 담긴 json을 받음
			var result = await RestAPI.GetAsync("https://api.github.com/repos/Nuwanda22/nuwanda22.github.io/contents/_posts/");

			// 성공일 경우
			if(result.IsSuccess)
			{
				// json을 파싱하여 글 목록을 만들고
				JArray array = JArray.Parse(result.Result);
				foreach (var item in array)
				{
					// 글 하나의 파일 이름 추출 후
					string fileName = item["name"].Value<string>();

					// 글 객체를 생성하여
					var newPost = new Post
					{
						Date = DateTime.Parse(fileName.Substring(0, 10)),
						Title = fileName.Substring(11).Replace('-', ' ').Split('.')[0]
					};

					// 리스트에 없는 경우 리스트에 추가한다.
					if (!list.Any(post => post.Title == newPost.Title && post.Date == newPost.Date))
					{
						list.Add(newPost);
					}
				}
			}

			// API의 성공 여부를 리턴함
			return result.IsSuccess;
		}

		private async void ContentPage_Appearing(object sender, EventArgs e)
		{
			if (isFirst)
			{
				Loading.IsVisible = true;
				Loading.IsRunning = true;

				await Load(PostList);

				Loading.IsVisible = false;
				Loading.IsRunning = false;

				isFirst = false;
			}
		}

		private void ToolbarItem_Clicked(object sender, EventArgs e)
		{
			Navigation.PushAsync(new PostingPage());
		}

		private void PostListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			(sender as ListView).SelectedItem = null;
		}

		private void MenuItem_Clicked(object sender, EventArgs e)
		{
			var item = sender as MenuItem;
			var param = item.CommandParameter as Post;

			DisplayAlert(item.Text, param.Title, "OK");
		}

		private async void PostListView_Refreshing(object sender, EventArgs e)
		{
			await Load(PostList);

			PostListView.IsRefreshing = false;
		}
	}

	public class Post
	{
		public string FileName { get; set; }
		public DateTime Date { get; set; }
		public string Title { get; set; }
	}
}
