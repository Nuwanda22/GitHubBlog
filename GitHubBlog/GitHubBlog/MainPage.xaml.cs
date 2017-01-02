using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;

namespace GitHubBlog
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
		}

        private async void OnClicked(object sender, EventArgs e)
        {
			using (HttpClient client = new HttpClient())
			{
				// 초기 설정
				client.MaxResponseContentBufferSize = 256000;
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 Safari/537.36");
				//client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "aa67fda2d3ad01095758c88029051e97b3fa3f1e");

				string url = $"https://api.github.com/repos/Nuwanda22/nuwanda22.github.io/contents/_posts/2016-11-24-github-pages.md";

				var responce = await client.GetAsync(new Uri(url));

				if (responce.IsSuccessStatusCode)
				{
					var content = await responce.Content.ReadAsStringAsync();

					await DisplayAlert("", content, "OK");
				}
				else
				{
					await DisplayAlert("Error", responce.StatusCode.ToString(), "OK");
				}
			}
		}
    }
}
