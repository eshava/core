namespace Eshava.Core.Validation.Attributes
{
	public class RangeToAttribute : AbstractRangeFromOrToAttribute
	{
		public RangeToAttribute(string propertyName, bool allowNull) : base(propertyName, allowNull)
		{

		}
	}
}