using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GitHubBlog
{
	static class RestAPI
	{
		public static string Key
		{
			get
			{
				return "a5143bb90d400201f8f152599034d09d85e0c8e0";
			}
		}

		public static int DataUsage { get; set; }

		public static async Task<HttpResult> GetAsync(string url)
		{
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "GitHub-Blog-App");

				var response = await client.GetAsync(url);
				AddDataUsage(response);

				return new HttpResult
				{
					IsSuccess = response.IsSuccessStatusCode,
					StatusCode = response.StatusCode,
					Result = await response.Content.ReadAsStringAsync()
				};
			}
		}

		public static async Task<HttpResult> PostAsync(string url, string json, string token)
		{
			using (HttpClient client = new HttpClient())
			{
				// 초기 설정
				client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "GitHub-Blog-App");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);

				var response = await client.PostAsync(new Uri(url), new StringContent(json));
				AddDataUsage(response);

				return new HttpResult
				{
					IsSuccess = response.IsSuccessStatusCode,
					StatusCode = response.StatusCode,
					Result = await response.Content.ReadAsStringAsync()
				};
			}
		}

		public static async Task<HttpResult> PutAsync(string url, string json, string token)
		{
			using (HttpClient client = new HttpClient())
			{
				// 초기 설정
				client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "GitHub-Blog-App");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);

				var response = await client.PutAsync(new Uri(url), new StringContent(json));
				AddDataUsage(response);

				return new HttpResult
				{
					IsSuccess = response.IsSuccessStatusCode,
					StatusCode = response.StatusCode,
					Result = await response.Content.ReadAsStringAsync()
				};
			}
		}

		private static void AddDataUsage(HttpResponseMessage response)
		{
			DataUsage += int.Parse(response.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());
		}
	}
}
