using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP
{
	public class PPItemBlock : PPItemBase
	{
		public Dal.SoheilEdmContext UOW { get; private set; }
		public Model.Block Model { get; private set; }
		public int[] ReportData { get; private set; }
		public bool CanAddSetupBefore { get; private set; }
		public int VIndex { get; set; }
		public DateTime ModifiedDate { get; private set; }
		public bool HasVm { get; set; }

		/// <summary>
		/// Creates an instance of <see cref="PPItemBlock"/> and reload its details
		/// <para>This class uses its own UOW</para>
		/// </summary>
		/// <param name="blockId"></param>
		public PPItemBlock(int blockId)
		{
			Id = blockId;
			HasVm = false;
			Reload();
		}
		/// <summary>
		/// Reloads the block full data keeping the current UOW
		/// </summary>
		public void Reload()
		{
			HasVm = false;

			if (UOW == null) 
				UOW = new Dal.SoheilEdmContext();
			var blockDataService = new DataServices.BlockDataService(UOW);
			Model = blockDataService.GetSingleFull(Id);
			
			ModifiedDate = Model.ModifiedDate;
			Start = Model.StartDateTime;
			End = Model.EndDateTime;

			ReportData = blockDataService.GetProductionReportData(Model);
			CanAddSetupBefore = blockDataService.CanAddSetupBeforeBlock(Model);
		}

		public static bool IsNull(PPItemBlock instance)
		{
			if (instance == null) return true;
			return (instance.Model == null);
		}

	}
}