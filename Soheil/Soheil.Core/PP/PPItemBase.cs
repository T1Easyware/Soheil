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
	}
}
