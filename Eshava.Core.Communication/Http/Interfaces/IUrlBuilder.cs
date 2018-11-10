using Eshava.Core.Communication.Models;

namespace Eshava.Core.Communication.Http.Interfaces
{
	public interface IUrlBuilder
	{
		string Build(UrlBuilderSettings settings);
		string GetSegementParameterName(string name);
	}
}