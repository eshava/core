using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Eshava.Core.Communication.Enums;
using Eshava.Core.Communication.Http.Interfaces;
using Eshava.Core.Communication.Models;
using Eshava.Core.Extensions;
using Newtonsoft.Json;

namespace Eshava.Core.Communication.Http
{
	public class RestHttpClient : IRestHttpClient
	{
		private readonly IUrlBuilder _urlBuiler;
		private readonly Uri _baseUrl;
		private readonly IHttpClient _httpClient;
		private DateTime _tokenExpires = DateTime.MinValue;

		public RestHttpClient(Uri baseUrl, IUrlBuilder urlBuiler = null, IHttpClient httpClient = null)
		{
			_baseUrl = baseUrl;
			_urlBuiler = urlBuiler ?? new UrlBuilder();
			_httpClient = httpClient ?? new SystemNetHttpClient();
			_httpClient.DefaultRequestHeaders?.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public object AuthorizationData { get; set; }
		public AuthorizationType AuthorizationType { get; set; }
		public string AuthorizationRouteSegment { get; set; }

		public HttpRequest CreateGet(params string[] segement)
		{
			return new HttpRequest(String.Join("/", segement), HttpMethod.Get);
		}

		public HttpRequest CreatePost(params string[] segement)
		{
			return new HttpRequest(String.Join("/", segement), HttpMethod.Post);
		}

		public HttpRequest CreatePut(params string[] segement)
		{
			return new HttpRequest(String.Join("/", segement), HttpMethod.Put);
		}

		public HttpRequest CreateDelete(params string[] segement)
		{
			return new HttpRequest(String.Join("/", segement), HttpMethod.Delete);
		}

		public async Task<HttpResponseMessage> SendAsync(HttpRequest request, object content = null)
		{
			var url = request.GetUrl(_urlBuiler, _baseUrl);
			var jsonContent = content != null ? GetJSONStringContent(content) : null;

			if (AuthorizationType == AuthorizationType.BearerToken && !await AddBearerTokenAsync())
			{
				return new HttpResponseMessage(HttpStatusCode.Unauthorized);
			}

			if (Equals(request.Method.Method, HttpMethod.Get.Method))
			{
				return await _httpClient.GetAsync(url);
			}

			if (Equals(request.Method.Method, HttpMethod.Post.Method))
			{
				return await _httpClient.PostAsync(url, jsonContent);
			}

			if (Equals(request.Method.Method, HttpMethod.Put.Method))
			{
				return await _httpClient.PutAsync(url, jsonContent);
			}

			if (Equals(request.Method.Method, HttpMethod.Delete.Method))
			{
				return await _httpClient.DeleteAsync(url);
			}

			throw new NotSupportedException("The http method is not supported: " + request.Method.Method);
		}

		public async Task<HttpResponseCustom<string>> GetContentAsync(HttpResponseMessage response, HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
		{
			if (response.StatusCode != expectedStatusCode)
			{
				return new HttpResponseCustom<string> { Result = null };
			}

			if (response.Content == null)
			{
				return new HttpResponseCustom<string> { Successful = true };
			}

			using (var content = response.Content)
			{
				return new HttpResponseCustom<string> { Successful = true, Result = await content.ReadAsStringAsync() };
			}
		}

		public async Task<HttpResponseCustom<T>> GetContentAsync<T>(HttpResponseMessage response, HttpStatusCode expectedStatusCode = HttpStatusCode.OK) where T : class
		{
			var result = await GetContentAsync(response, expectedStatusCode);

			if (response.StatusCode == expectedStatusCode)
			{
				return new HttpResponseCustom<T> { Successful = true, Result = result.Result.IsNullOrEmpty() ? default(T) : JsonConvert.DeserializeObject<T>(result.Result) };
			}

			return new HttpResponseCustom<T> { Successful = false, Result = default(T) };
		}

		public string GetSegementParameterName(string name)
		{
			return _urlBuiler.GetSegementParameterName(name);
		}

		private async Task<bool> AddBearerTokenAsync()
		{
			if (AuthorizationData == null)
			{
				throw new ArgumentNullException(nameof(AuthorizationData));
			}

			if (!Equals(_tokenExpires, DateTime.MinValue) && _tokenExpires.Subtract(new TimeSpan(0, 5, 0)) > DateTime.UtcNow)
			{
				return true;
			}

			var url = new UrlBuilder().Build(new UrlBuilderSettings { BaseUrl = _baseUrl, Segment = AuthorizationRouteSegment });

			using (var response = await _httpClient.PostAsync(url, GetJSONStringContent(AuthorizationData)))
			{
				if (!response.IsSuccessStatusCode)
				{
					return false;
				}

				using (var content = response.Content)
				{
					var result = await content.ReadAsStringAsync();

					if (result.IsNullOrEmpty())
					{
						return false;
					}

					var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(result);
					_tokenExpires = tokenResponse.Expires.ToUniversalTime();

					if (_httpClient.DefaultRequestHeaders != null)
					{
						_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);
					}

					return true;
				}
			}
		}

		private static StringContent GetJSONStringContent(object content)
		{
			return new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
		}
	}
}