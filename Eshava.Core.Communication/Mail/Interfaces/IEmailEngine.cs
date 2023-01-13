using System.Threading.Tasks;
using Eshava.Core.Communication.Models;
using Eshava.Core.Models;

namespace Eshava.Core.Communication.Mail.Interfaces
{
	public interface IEmailEngine
	{
		string Type { get; }
		Task<ResponseData<bool>> SendMailAsync(EmailData mail);
		Task<ResponseData<bool>> SendMailAsync(EmailData mail,  EmailAccount emailAccount);
	}
}