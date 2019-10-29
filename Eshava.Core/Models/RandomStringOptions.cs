namespace Eshava.Core.Models
{
	public class RandomStringOptions
	{
		public bool IncludeNumbers { get; set; }
		public bool IncludeUppercase { get; set; }
		public bool IncludeLowercase { get; set; }
		public bool IncludeSpecialCharacter { get; set; }
		public bool NoCharacterTypeEnabled => !IncludeNumbers && !IncludeUppercase && !IncludeLowercase && !IncludeSpecialCharacter;
	}
}