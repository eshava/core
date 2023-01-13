# core
Collection of methods, classes and helpers to simplify recurring standard actions

## core.linq
Extension for dynamic creation of filter and sort queries based on IQueryable
* WhereQueryEngine
* SortingQueryEngine

Extension for transformation of expression tree from one data type to an other data type
* TransformQueryEngine
* TransformProfile

```csharp
public IEnumerable<Target> ReadSomething(Expression<Func<Source, bool>> sourcExpression)
{
	var targetData = new List<Target>();
	var engine = new TransformQueryEngine();
	
	var targetExpression = engine.Transform<Source, Target>(sourcExpression);
	
	var result = targetData
					.Where(targetExpression.Compile())
					.ToList()
					;
			
	return result;
}
```

```csharp
public enum CustomTransformProfile : TransformProfile
{
	public CustomTransformProfile()
	{
		CreateMap<Source, Target>()
			.ForPath(s => s.SourceId, t => t.TargetId)
			;
	}
}
```