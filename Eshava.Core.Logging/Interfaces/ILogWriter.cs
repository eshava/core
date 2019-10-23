using Eshava.Core.Logging.Models;

namespace Eshava.Core.Logging.Interfaces
{
	public interface ILogWriter
	{
		void Write(LogEntry logEntry);
	}
}