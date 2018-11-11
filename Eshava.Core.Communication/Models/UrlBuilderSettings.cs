using System;
using System.Collections.Generic;

namespace Eshava.Core.Communication.Models
{
	public class UrlBuilderSettings
	{
		public UrlBuilderSettings()
		{
			SegmentParameter = new Dictionary<string, object>();
			QueryParameter = new List<(string Name, object Value)>();
		}

		public Uri BaseUrl { get; set; }
		public string Segment { get; set; }
		public IDictionary<string, object> SegmentParameter { get; set; }
		public IEnumerable<(string Name, object Value)> QueryParameter { get; set; }
	}
}