using System.Threading.Tasks;
using Eshava.Core.Communication.Models;
using Eshava.Core.Models;

namespace Eshava.Core.Communication.Email.Interfaces
{
	public interface IEmailEngine
	{
		string Type { get; }
		Task<ResponseData<bool>> SendEmailAsync(EmailData emailData);
		Task<ResponseData<bool>> SendEmailAsync(EmailData emailData, EmailAccount emailAccount);
	}
}