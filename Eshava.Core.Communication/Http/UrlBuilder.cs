using System;
using System.Linq;
using System.Text;
using Eshava.Core.Communication.Http.Interfaces;
using Eshava.Core.Communication.Models;
using Eshava.Core.Extensions;

namespace Eshava.Core.Communication.Http
{
	public class UrlBuilder : IUrlBuilder
	{
		public string Build(UrlBuilderSettings settings)
		{
			if (settings.BaseUrl == null)
			{
				throw new ArgumentNullException(nameof(settings.BaseUrl));
			}

			var url = new StringBuilder();
			url.Append(settings.BaseUrl.AbsoluteUri);

			if (!settings.BaseUrl.AbsoluteUri.EndsWith("/"))
			{
				url.Append("/");
			}

			var segement = settings.Segment ?? "";
			if (settings.SegmentParameter != null && settings.SegmentParameter.Count > 0)
			{
				foreach (var key in settings.SegmentParameter.Keys)
				{
					segement = segement.Replace(GetSegementParameterName(key), settings.SegmentParameter[key].ToString());
				}
			}

			if (!segement.IsNullOrEmpty())
			{
				url.Append(segement);

				if (!segement.EndsWith("/"))
				{
					url.Append("/");
				}
			}

			if (settings.QueryParameter != null && settings.QueryParameter.Any())
			{
				url.Append("?");
				url.Append(String.Join("&", settings.QueryParameter.Select(p => p.Name + "=" + p.Value.ToString())));
			}

			var totalUrl = new Uri(url.ToString());

			return totalUrl.AbsoluteUri;
		}

		public string GetSegementParameterName(string name)
		{
			if (name.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(name));
			}

			return "{" + name + "}";
		}
	}
}