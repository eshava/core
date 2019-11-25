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
		public IList<WhereQueryProperty> WhereQueryProperties { get; set; }
		public IList<SortingQueryProperty> SortingQueryProperties { get; set; }
	}
}