using Eshava.Core.Linq.Interfaces;
using Eshava.Core.Linq.Models;

namespace Eshava.Core.Linq
{
	public abstract class TransformProfile
	{
		public IMappingExpression<Source, Target> CreateMap<Source, Target>()
		{
			var mappingExpression = new MappingExpression<Source, Target>();
			
			MappingStore.Mappings.Add(mappingExpression);

			return mappingExpression;
		}
	}
}