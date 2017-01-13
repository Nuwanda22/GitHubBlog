using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Rg.Plugins.Popup.Extensions;

namespace GitHubBlog
{
	public partial class LoginPage : ContentPage
	{
		bool IsAfterLogout;

		public LoginPage(bool isAfterLogout)
		{
			InitializeComponent();

			IsAfterLogout = isAfterLogout;
		}

		public void Loading(bool loading)
		{
			ButtonGrid.IsEnabled = !loading;
			Indicator.IsVisible = loading;
		}

		private async void Button_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushPopupAsync(new LoginPopupPage(IsAfterLogout));
		}
	}
}
