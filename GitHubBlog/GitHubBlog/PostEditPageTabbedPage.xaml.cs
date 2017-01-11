using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace GitHubBlog
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PostEditPageTabbedPage : TabbedPage
	{
		public PostEditPageTabbedPage()
		{
			InitializeComponent();
		}
	}
}
