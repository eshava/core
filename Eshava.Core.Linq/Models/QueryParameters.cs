using System.Collections.Generic;

namespace Eshava.Core.Linq.Models
{
	public class QueryParameters
	{
		public QueryParameters()
		{
			WhereQueryProperties = new List<WhereQueryProperty>();
			SortingQueryProperties = new List<SortingQueryProperty>();
		}

		public string SearchTerm { get; set; }
		public IEnumerable<WhereQueryProperty> WhereQueryProperties { get; set; }
		public IEnumerable<SortingQueryProperty> SortingQueryProperties { get; set; }
	}
}