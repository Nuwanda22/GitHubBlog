using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;

namespace GitHubBlog
{
	public partial class LoginSuccessPopupPage : PopupPage
	{
		public LoginSuccessPopupPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			HidePopup();
		}

		private async void HidePopup()
		{
			await Task.Delay(4000);
			await PopupNavigation.RemovePageAsync(this);
		}
	}
}
