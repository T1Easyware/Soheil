using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.PP
{
	public class ItemMetaInfo
	{
		/// <summary>
		/// Creates a new ItemMetaInfo for the given data
		/// </summary>
		/// <param name="id">Id of model</param>
		/// <param name="idx">Index of Station</param>
		/// <param name="modDate">Modified Date</param>
		public ItemMetaInfo(int id, int idx, DateTime modDate)
		{
			Id = id;
			StationIndex = idx;
			ModifiedDate = modDate;
		}
		public int Id;
		public int StationIndex;
		public DateTime ModifiedDate;
	}
}
