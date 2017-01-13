using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;


namespace GitHubBlog
{
	public partial class YamlEditPopupPage : PopupPage
	{
		Dictionary<object, object> YamlDictionary;

		public YamlEditPopupPage(Dictionary<object, object> yamlDictionary)
		{
			InitializeComponent();
			YamlDictionary = yamlDictionary;
			listView.ItemsSource = YamlDictionary;
		}

		protected override bool OnBackgroundClicked()
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				await Navigation.PopAllPopupAsync();
			});

			return false;
		}

		private async void OnClose(object sender, EventArgs e)
		{
			await Navigation.PopAllPopupAsync();
		}

		private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			(sender as ListView).SelectedItem = null;
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			
		}
	}
}
