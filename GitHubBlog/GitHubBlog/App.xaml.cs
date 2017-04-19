using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace GitHubBlog
{
	public partial class App : Application
	{
		static App current;

		public App()
		{
			InitializeComponent();

			current = this;

			// 이전에 로그인했다면 메인 페이지를 띄운다.
			if (Properties.ContainsKey("token"))
			{
				MainPage = new NavigationPage(new PostListPage());
			}
			// 그게 아니라면 로그인 페이지를 띄운다.
			else
			{
				MainPage = new LoginPage(false);
			}
		}

		public static new App Current
		{
			get
			{
				return current;
			}
		}
	}
}
