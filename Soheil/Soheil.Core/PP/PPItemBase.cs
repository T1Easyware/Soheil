using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using Soheil.Common;

namespace Soheil.Core.PP
{
	public class PPItemBase
	{

		protected PPItemBase() { }

		/// <summary>
		/// Gets or sets a value for start of this time range
		/// </summary>
		public DateTime Start { get; protected set; }
		/// <summary>
		/// Gets or sets a value for end of this time range
		/// </summary>
		public DateTime End { get; protected set; }

		/// <summary>
		/// Id of item's model
		/// </summary>
		public int Id { get; protected set; }
		public int VIndex { get; set; }
		public bool HasVm { get; set; }
		public DateTime ModifiedDate { get; protected set; }

		public static bool IsNull(PPItemBase instance)
		{
			if (instance == null) return true;
			if (instance is PPItemBlock)
				return ((instance as PPItemBlock).Model == null);
			if (instance is PPItemNpt)
				return ((instance as PPItemNpt).Model == null);
			return true;
		}
	}
}
