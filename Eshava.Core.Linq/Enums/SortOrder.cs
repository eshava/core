using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Eshava.Core.Linq.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum SortOrder
	{
		[EnumMember(Value = nameof(None))]
		None = 0,
		[EnumMember(Value = "Asc")]
		Ascending = 1,
		[EnumMember(Value = "Desc")]
		Descending = 2
	}
}