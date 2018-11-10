using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eshava.Core.Communication.Enums;
using Eshava.Core.Communication.Models;

namespace Eshava.Core.Communication.Http.Interfaces
{
	public interface IRestHttpClient
	{
		object AuthorizationData { get; set; }
		AuthorizationType AuthorizationType { get; set; }

		HttpRequest CreateGet(params string[] segement);
		HttpRequest CreatePost(params string[] segement);
		HttpRequest CreatePut(params string[] segement);
		HttpRequest CreateDelete(params string[] segement);

		Task<HttpResponseMessage> SendAsync(HttpRequest request, object content = null);
		Task<HttpResponseCustom<string>> GetContentAsync(HttpResponseMessage response, HttpStatusCode expectedStatusCode = HttpStatusCode.OK);
		Task<HttpResponseCustom<T>> GetContentAsync<T>(HttpResponseMessage response, HttpStatusCode expectedStatusCode = HttpStatusCode.OK) where T : class;

		string GetSegementParameterName(string name);
	}
}