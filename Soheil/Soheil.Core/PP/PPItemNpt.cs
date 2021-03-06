﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.PP
{
	public class PPItemNpt : PPItemBase
	{
		public Dal.SoheilEdmContext UOW { get; private set; }
		public Model.NonProductiveTask Model { get; protected set; }

		/// <summary>
		/// Creates an instance of <see cref="PPItemNpt"/> for given NonProductiveTask
		/// <para>This class uses its own UOW</para>
		/// </summary>
		public PPItemNpt(int nptId)
		{
			Id = nptId;
			HasVm = false;
			Reload();
		}
		/// <summary>
		/// Reloads the NonProductiveTask full data keeping the current UOW
		/// </summary>
		public void Reload()
		{
			HasVm = false;

			if (UOW == null)
				UOW = new Dal.SoheilEdmContext();
			var nptDataService = new DataServices.NPTDataService(UOW);

			ModifiedDate = DateTime.MinValue;//???
			Model = nptDataService.GetSingle(Id);
			Start = Model.StartDateTime;
			End = Model.EndDateTime;
		}

	}
}
