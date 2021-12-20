using System.Collections.Generic;
using Eshava.Core.Linq.Interfaces;

namespace Eshava.Core.Linq.Models
{
	internal static class MappingStore
	{
		static MappingStore()
		{
			Mappings = new List<IMappingExpression>();
		}
				
		internal static IList<IMappingExpression> Mappings { get; }
	}
}