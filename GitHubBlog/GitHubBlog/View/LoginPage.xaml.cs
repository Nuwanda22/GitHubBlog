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
		public LoginPage()
		{
			InitializeComponent();
		}

		public void Loading()
		{
			LoginButton.IsEnabled = false;
			Indicator.IsVisible = true;
		}

		private async void Button_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushPopupAsync(new LoginPopupPage());
		}
	}
}
