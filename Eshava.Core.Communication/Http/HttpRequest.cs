using System;
using System.Collections.Generic;
using System.Net.Http;
using Eshava.Core.Communication.Http.Interfaces;
using Eshava.Core.Communication.Models;

namespace Eshava.Core.Communication.Http
{
	public class HttpRequest
	{
		private readonly Dictionary<string, object> _segmentParameter = new Dictionary<string, object>();
		private readonly List<(string Name, object Value)> _queryParameter = new List<(string Name, object Value)>();

		public HttpRequest(string segement, HttpMethod method)
		{
			Segement = segement;
			Method = method;
		}

		public HttpMethod Method { get; }
		public string Segement { get; }

		public void AddSegmentParameter(string name, object value)
		{
			if (!_segmentParameter.ContainsKey(name))
			{
				_segmentParameter.Add(name, value);
			}
			else
			{
				_segmentParameter[name] = value;
			}
		}

		public void AddQueryParameter(string name, object value)
		{
			_queryParameter.Add((name, value));
		}

		public string GetUrl(IUrlBuilder urlBuilder, Uri baseUrl)
		{
			var settings = new UrlBuilderSettings
			{
				BaseUrl = baseUrl,
				Segment = Segement,
				SegmentParameter = _segmentParameter,
				QueryParameter = _queryParameter
			};

			return urlBuilder.Build(settings);
		}
	}
}