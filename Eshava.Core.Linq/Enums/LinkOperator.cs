using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Eshava.Core.Linq.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum LinkOperator
	{
		[EnumMember(Value = nameof(None))]
		None = 0,
		[EnumMember(Value = nameof(And))]
		And = 1,
		[EnumMember(Value = nameof(Or))]
		Or = 2,
	}
}