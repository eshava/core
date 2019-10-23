using System.Collections.Generic;
using Eshava.Core.Logging.Models;

namespace Eshava.Core.Logging.Interfaces
{
	public interface IDataRecordLogEngine
	{
		IEnumerable<DataRecordLogEntry<T>> Convert<T>(IEnumerable<DataRecordLogProperty<T>> dataRecordLogProperties);
	}
}