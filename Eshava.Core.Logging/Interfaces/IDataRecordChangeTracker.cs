using System.Collections.Generic;
using Eshava.Core.Logging.Models;
using System.ComponentModel.DataAnnotations;

namespace Eshava.Core.Logging.Interfaces
{
	public interface IDataRecordChangeTracker<P>
	{
		IEnumerable<DataRecordLogProperty<P>> CreateInsertLogs<T>(T dataRecord, P dataRecordId, P dataRecordParentId) where T : class;
		IEnumerable<DataRecordLogProperty<P>> CreateUpdateLogs<T>(T dataRecord, T dataRecordToCompare, P dataRecordId, P dataRecordParentId) where T : class;

		/// <summary>
		/// To create an deletion log the <see cref="KeyAttribute"/> must be set on one property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="dataRecordId"></param>
		/// <param name="dataRecordParentId"></param>
		/// <returns></returns>
		DataRecordLogProperty<P> CreateDeleteLog<T>(P dataRecordId, P dataRecordParentId) where T : class;
	}
}