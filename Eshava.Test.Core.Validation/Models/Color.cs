﻿using System.Runtime.Serialization;

namespace Eshava.Test.Core.Validation.Models
{
	public enum Color : short
	{
		[EnumMember(Value = "transparent")]
		Transparent,
		[EnumMember(Value = "black")]
		Black,
		[EnumMember(Value = "white")]
		White,
		[EnumMember(Value = "purple")]
		Purple
	}
}