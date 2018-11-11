using System;

namespace Eshava.Core.Communication.Models
{
	public class TokenResponse
	{
        public string Token { get; set; }
        public DateTime Expires { get; set; }
	}
}