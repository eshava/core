using System.Threading.Tasks;
using Eshava.Core.Communication.Models;

namespace Eshava.Core.Communication.Mail.Interfaces
{
	public interface IMailEngine
	{
		string Type { get; }
		Task<IMailEngine> SendMailAsync(MailData mail);
		Task<IMailEngine> SendMailAsync(MailData mail, string sender, string password);
	}
}