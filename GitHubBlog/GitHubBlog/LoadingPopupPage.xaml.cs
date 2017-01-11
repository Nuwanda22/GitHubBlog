using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Rg.Plugins.Popup.Pages;

namespace GitHubBlog
{
	public partial class LoadingPopupPage : PopupPage
	{
		public LoadingPopupPage()
		{
			InitializeComponent();
		}

		protected override bool OnBackButtonPressed()
		{
			return true;
		}
	}
}
