namespace Eshava.Core.Validation.Attributes
{
	public class RangeFromAttribute : AbstractRangeFromOrToAttribute
	{
		public RangeFromAttribute(string propertyName, bool allowNull) : base(propertyName, allowNull)
		{

		}
	}
}