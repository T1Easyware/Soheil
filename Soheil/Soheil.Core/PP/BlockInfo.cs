using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.PP
{
	public class BlockInfo
	{
		public BlockInfo(int id, int idx, DateTime modDate)
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
