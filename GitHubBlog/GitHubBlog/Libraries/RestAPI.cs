using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

using GitHubBlog.Models;

namespace GitHubBlog.Libraries
{
	static class RestAPI
	{
		public static async Task<HttpResult> GetAsync(string url, Dictionary<string,string> headers = null, string token = null)
		{
			using (HttpClient client = new HttpClient())
			{
				client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "GitHub-Blog-App");
				if(token != null) client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
				if (headers != null) foreach (var header in headers)
				{
					client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
				}

				var response = await client.GetAsync(url);

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

				return new HttpResult
				{
					IsSuccess = response.IsSuccessStatusCode,
					StatusCode = response.StatusCode,
					Result = await response.Content.ReadAsStringAsync()
				};
			}
		}

		public static async Task<HttpResult> DeleteAsync(string url, string json, string token)
		{
			using (HttpClient client = new HttpClient())
			{
				// 초기 설정
				client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "GitHub-Blog-App");
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
				
				var response = await client.SendAsync(new HttpRequestMessage
				{
					Content = new StringContent(json),
					Method = HttpMethod.Delete,
					RequestUri = new Uri(url)
				});

				return new HttpResult
				{
					IsSuccess = response.IsSuccessStatusCode,
					StatusCode = response.StatusCode,
					Result = await response.Content.ReadAsStringAsync()
				};
			}
		}
	}
}
