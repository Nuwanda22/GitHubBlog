using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Newtonsoft.Json;

namespace GitHubBlog.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PostEditPage : TabbedPage
	{
		EditPage EditingPage;

		public PostEditPage(bool isNew)
		{
			InitializeComponent();

			EditingPage = new EditPage(isNew);
			Children.Insert(0, EditingPage);
			CurrentPage = EditingPage;

			if (isNew)
			{
				Title = "Post";
			}
			else
			{
				Title = "Edit";
			}
		}

		protected override void OnCurrentPageChanged()
		{
			base.OnCurrentPageChanged();

			if (EditingPage == null) return;
			if (CurrentPage.Title == "Preview")
			{
				// TODO: async + ActivityIndicator
				Previewer.Source = new HtmlWebViewSource { Html = CommonMark.CommonMarkConverter.Convert(EditingPage.EditorText) };
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
