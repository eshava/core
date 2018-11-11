using Eshava.Core.Validation.Models;

namespace Eshava.Core.Validation.Interfaces
{
	public interface IValidationEngine
	{
		ValidationCheckResult Validate(object model);
	}
}