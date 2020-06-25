namespace Eshava.Core.Linq.Models
{
	public class WhereQueryEngineOptions
	{
		public bool UseUtcDateTime { get; set; }

		/// <summary>
		/// The search term is separated by spaces. Each search term is ORed across all properties. At the end all OR conditions are combined with AND.
		/// </summary>
		public bool ContainsSearchSplitBySpace { get; set; }
	}
}