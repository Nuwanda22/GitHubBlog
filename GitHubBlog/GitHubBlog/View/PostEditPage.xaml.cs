using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Newtonsoft.Json;

namespace GitHubBlog
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PostEditPage : TabbedPage
	{
		public PostEditPage()
		{
			InitializeComponent();
			
		}

		protected override async void OnCurrentPageChanged()
		{
			base.OnCurrentPageChanged();

			if(CurrentPage.Title == "Priview")
			{
				// 바뀐 글을 가져와 API를 요청함
				var result = await RestAPI.PostAsync("https://api.github.com/markdown", JsonConvert.SerializeObject(new
				{
					text = EditingPage.EditorText
				}), RestAPI.Key);

				if (result.IsSuccess)
				{
					Previewer.Source = new HtmlWebViewSource { Html = result.Result };
				}
			}
		}

		protected override bool OnBackButtonPressed()
		{
			if (CurrentPage.Title == "Priview")
			{
				// TODO: EditPage로 스위칭하기
				return true;
			}
			else
			{
				Device.BeginInvokeOnMainThread(async() =>
				{
					if(await DisplayAlert("변경된 내용은 저장되지 않습니다", "정말 나가시겠습니까?", "Yes", "No"))
					{
						await Navigation.PopAsync();
					}
				});
				return true;
			}
		}
	}
}
