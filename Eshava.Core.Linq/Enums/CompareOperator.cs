using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Eshava.Core.Linq.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum CompareOperator
	{
		[EnumMember(Value = nameof(None))]
		None = 0,
		[EnumMember(Value = nameof(Equals))]
		Equals = 1,
		[EnumMember(Value = nameof(NotEquals))]
		NotEquals = 2,
		[EnumMember(Value = nameof(GreaterThan))]
		GreaterThan = 3,
		[EnumMember(Value = nameof(GreaterThanOrEqual))]
		GreaterThanOrEqual = 4,
		[EnumMember(Value = nameof(LessThan))]
		LessThan = 5,
		[EnumMember(Value = nameof(LessThanOrEqual))]
		LessThanOrEqual = 6,
		[EnumMember(Value = nameof(Contains))]
		Contains = 7
	}
}