using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eshava.Core.Communication.Enums;
using Eshava.Core.Communication.Http;
using Eshava.Core.Communication.Http.Interfaces;
using Eshava.Core.Communication.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Eshava.Test.Core.Communication.Http
{
	[TestClass, TestCategory("Core.Communication")]
	public class RestHttpClientTest
	{
		private RestHttpClient _classUnderTest;
		private IHttpClient _httpClient;

		[TestInitialize]
		public void Setup()
		{
			_httpClient = A.Fake<IHttpClient>();
			_classUnderTest = new RestHttpClient(new Uri("https://eshava.de/"), httpClient: _httpClient);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void GetSegementParameterNameWithNullInputTest()
		{
			// Act
			_classUnderTest.GetSegementParameterName(null);
		}

		[TestMethod]
		public void CreateGetWithoutSegmentTest()
		{
			// Act
			var result = _classUnderTest.CreateGet();

			// Assert
			result.Segement.Should().Be("");
			result.Method.Should().Be(HttpMethod.Get);
		}

		[TestMethod]
		public void CreateGetWithSegmentTest()
		{
			// Act
			var result = _classUnderTest.CreateGet("darkwing", "duck");

			// Assert
			result.Segement.Should().Be("darkwing/duck");
			result.Method.Should().Be(HttpMethod.Get);
		}

		[TestMethod]
		public void CreatePostWithoutSegmentTest()
		{
			// Act
			var result = _classUnderTest.CreatePost();

			// Assert
			result.Segement.Should().Be("");
			result.Method.Should().Be(HttpMethod.Post);
		}

		[TestMethod]
		public void CreatePostWithSegmentTest()
		{
			// Act
			var result = _classUnderTest.CreatePost("darkwing", "duck");

			// Assert
			result.Segement.Should().Be("darkwing/duck");
			result.Method.Should().Be(HttpMethod.Post);
		}

		[TestMethod]
		public void CreatePutWithoutSegmentTest()
		{
			// Act
			var result = _classUnderTest.CreatePut();

			// Assert
			result.Segement.Should().Be("");
			result.Method.Should().Be(HttpMethod.Put);
		}

		[TestMethod]
		public void CreatePutWithSegmentTest()
		{
			// Act
			var result = _classUnderTest.CreatePut("darkwing", "duck");

			// Assert
			result.Segement.Should().Be("darkwing/duck");
			result.Method.Should().Be(HttpMethod.Put);
		}

		[TestMethod]
		public void CreateDeleteWithoutSegmentTest()
		{
			// Act
			var result = _classUnderTest.CreateDelete();

			// Assert
			result.Segement.Should().Be("");
			result.Method.Should().Be(HttpMethod.Delete);
		}

		[TestMethod]
		public void CreateDeleteWithSegmentTest()
		{
			// Act
			var result = _classUnderTest.CreateDelete("darkwing", "duck");

			// Assert
			result.Segement.Should().Be("darkwing/duck");
			result.Method.Should().Be(HttpMethod.Delete);
		}

		[TestMethod, ExpectedException(typeof(AggregateException))]
		public void SendWithInvalidHttpMethodTest()
		{
			// Arrange
			var request = new HttpRequest("data", new HttpMethod("DarkwingDuck"));

			// Act
			var task = _classUnderTest.SendAsync(request);
			task.Exception.InnerException.GetType().Should().Be(typeof(NotSupportedException));
			task.Wait();
		}

		[TestMethod]
		public void SendAsyncTryGetAuthorizationHeaderResultUnauthorizedTest()
		{
			// Arrange
			var request = _classUnderTest.CreateGet("quackfu");
			_classUnderTest.AuthorizationType = AuthorizationType.BearerToken;
			_classUnderTest.AuthorizationData = new { Username = "darkwing", Password = "duck" };

			A.CallTo(() => _httpClient.PostAsync(A<string>.Ignored, A<HttpContent>.Ignored)).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)));

			// Act
			var task = _classUnderTest.SendAsync(request);
			task.Wait();

			// Assert
			A.CallTo(() => _httpClient.PostAsync(A<string>.Ignored, A<HttpContent>.Ignored)).MustHaveHappenedOnceExactly();
			task.Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		}

		[TestMethod]
		public void SendAsyncTryGetAuthorizationHeaderResultOkTest()
		{
			// Arrange
			var request = _classUnderTest.CreateGet("quackfu");
			_classUnderTest.AuthorizationType = AuthorizationType.BearerToken;
			_classUnderTest.AuthorizationData = new { Username = "darkwing", Password = "duck" };

			var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(JsonConvert.SerializeObject(new TokenResponse { Token = "bearerToken" }))
			};

			A.CallTo(() => _httpClient.PostAsync(A<string>.Ignored, A<HttpContent>.Ignored)).Returns(Task.FromResult(responseMessage));

			// Act
			var task = _classUnderTest.SendAsync(request);
			task.Wait();

			// Assert
			A.CallTo(() => _httpClient.PostAsync(A<string>.Ignored, A<HttpContent>.Ignored)).MustHaveHappenedOnceExactly();
			task.Result.StatusCode.Should().Be(HttpStatusCode.OK);
		}


		[TestMethod]
		public void SendAsyncGetTest()
		{
			// Arrange
			var request = _classUnderTest.CreateGet("darkwing/duck");
			var url = "";

			A.CallTo(() => _httpClient.GetAsync(A<string>.Ignored)).Invokes(call =>
			{
				url = call.GetArgument<string>(0);
			});

			// Act
			var task = _classUnderTest.SendAsync(request);
			task.Wait();

			// Assert
			A.CallTo(() => _httpClient.GetAsync(A<string>.Ignored)).MustHaveHappened();
			A.CallTo(() => _httpClient.PostAsync(A<string>.Ignored, A<HttpContent>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => _httpClient.PutAsync(A<string>.Ignored, A<HttpContent>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => _httpClient.DeleteAsync(A<string>.Ignored)).MustNotHaveHappened();

			url.Should().Be("https://eshava.de/darkwing/duck/");
		}

		[TestMethod]
		public void SendAsyncPostTest()
		{
			// Arrange
			var request = _classUnderTest.CreatePost("darkwing/duck");
			var data = new TokenResponse { Token = "QuackFu" };
			var url = "";
			StringContent content = null;

			A.CallTo(() => _httpClient.PostAsync(A<string>.Ignored, A<HttpContent>.Ignored)).Invokes(call =>
			{
				url = call.GetArgument<string>(0);
				content = call.GetArgument<StringContent>(1);
			});

			// Act
			var task = _classUnderTest.SendAsync(request, data);
			task.Wait();

			// Assert
			A.CallTo(() => _httpClient.GetAsync(A<string>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => _httpClient.PostAsync(A<string>.Ignored, A<HttpContent>.Ignored)).MustHaveHappened();
			A.CallTo(() => _httpClient.PutAsync(A<string>.Ignored, A<HttpContent>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => _httpClient.DeleteAsync(A<string>.Ignored)).MustNotHaveHappened();

			url.Should().Be("https://eshava.de/darkwing/duck/");
			JsonConvert.DeserializeObject<TokenResponse>(content.ReadAsStringAsync().Result).Token.Should().Be("QuackFu");
		}

		[TestMethod]
		public void SendAsyncPutTest()
		{
			// Arrange
			var request = _classUnderTest.CreatePut("darkwing/duck");
			var data = new TokenResponse { Token = "QuackFu" };
			var url = "";
			StringContent content = null;

			A.CallTo(() => _httpClient.PutAsync(A<string>.Ignored, A<HttpContent>.Ignored)).Invokes(call =>
			{
				url = call.GetArgument<string>(0);
				content = call.GetArgument<StringContent>(1);
			});

			// Act
			var task = _classUnderTest.SendAsync(request, data);
			task.Wait();

			// Assert
			A.CallTo(() => _httpClient.GetAsync(A<string>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => _httpClient.PostAsync(A<string>.Ignored, A<HttpContent>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => _httpClient.PutAsync(A<string>.Ignored, A<HttpContent>.Ignored)).MustHaveHappened();
			A.CallTo(() => _httpClient.DeleteAsync(A<string>.Ignored)).MustNotHaveHappened();

			url.Should().Be("https://eshava.de/darkwing/duck/");
			JsonConvert.DeserializeObject<TokenResponse>(content.ReadAsStringAsync().Result).Token.Should().Be("QuackFu");
		}

		[TestMethod]
		public void SendAsyncDeleteTest()
		{
			// Arrange
			var request = _classUnderTest.CreateDelete("darkwing/duck");
			var url = "";

			A.CallTo(() => _httpClient.DeleteAsync(A<string>.Ignored)).Invokes(call =>
			{
				url = call.GetArgument<string>(0);
			});

			// Act
			var task = _classUnderTest.SendAsync(request);
			task.Wait();

			// Assert
			A.CallTo(() => _httpClient.GetAsync(A<string>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => _httpClient.PostAsync(A<string>.Ignored, A<HttpContent>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => _httpClient.PutAsync(A<string>.Ignored, A<HttpContent>.Ignored)).MustNotHaveHappened();
			A.CallTo(() => _httpClient.DeleteAsync(A<string>.Ignored)).MustHaveHappened();

			url.Should().Be("https://eshava.de/darkwing/duck/");
		}

		[TestMethod]
		public void GetSegementParameterNameTest()
		{
			// Act
			var result = _classUnderTest.GetSegementParameterName("QuackFu");

			// Assert
			result.Should().Be("{QuackFu}");
		}

		[TestMethod]
		public void GetContentStringAsyncNotContentTest()
		{
			// Arrange
			var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

			// Act
			var task = _classUnderTest.GetContentAsync(responseMessage);
			task.Wait();

			// Assert
			task.Result.Successful.Should().BeTrue();
			task.Result.Result.Should().BeNull();
		}

		[TestMethod]
		public void GetContentStringAsyncStringContentTest()
		{
			// Arrange
			var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("Darkwing Duck")
			};

			// Act
			var task = _classUnderTest.GetContentAsync(responseMessage);
			task.Wait();

			// Assert
			task.Result.Successful.Should().BeTrue();
			task.Result.Result.Should().Be("Darkwing Duck");
		}

		[TestMethod]
		public void GetContentStringAsyncBadRequestTest()
		{
			// Arrange
			var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
			{
				Content = new StringContent("Darkwing Duck")
			};

			// Act
			var task = _classUnderTest.GetContentAsync(responseMessage);
			task.Wait();

			// Assert
			task.Result.Successful.Should().BeFalse();
			task.Result.Result.Should().BeNull();
		}

		[TestMethod]
		public void GetContentStringAsyncClassContentTest()
		{
			// Arrange
			var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(JsonConvert.SerializeObject(new TokenResponse { Token = "Darkwing Duck" }))
			};

			// Act
			var task = _classUnderTest.GetContentAsync<TokenResponse>(responseMessage);
			task.Wait();

			// Assert
			task.Result.Successful.Should().BeTrue();
			task.Result.Result.Token.Should().Be("Darkwing Duck");
		}

		[TestMethod]
		public void GetContentStringAsyncClassContentBadRequestTest()
		{
			// Arrange
			var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
			{
				Content = new StringContent(JsonConvert.SerializeObject(new TokenResponse { Token = "Darkwing Duck" }))
			};

			// Act
			var task = _classUnderTest.GetContentAsync<TokenResponse>(responseMessage);
			task.Wait();

			// Assert
			task.Result.Successful.Should().BeFalse();
			task.Result.Result.Should().BeNull();
		}
	}
}