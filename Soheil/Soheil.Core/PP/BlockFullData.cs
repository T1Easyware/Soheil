using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP
{
	public class BlockFullData
	{
		public Dal.SoheilEdmContext UOW { get; private set; }
		public BlockFullData(int blockId)
		{
			UOW = new Dal.SoheilEdmContext();
			var blockDataService = new DataServices.BlockDataService(UOW);
			Model = blockDataService.GetSingleFull(blockId);
			ReportData = blockDataService.GetProductionReportData(Model);
			CanAddSetupBefore = blockDataService.CanAddSetupBeforeBlock(Model);
		}
		/// <summary>
		/// Reloads the block full data keeping the current UOW
		/// </summary>
		public void Reload()
		{
			var blockDataService = new DataServices.BlockDataService(UOW);
			Model = blockDataService.GetSingleFull(Model.Id);
			ReportData = blockDataService.GetProductionReportData(Model);
			CanAddSetupBefore = blockDataService.CanAddSetupBeforeBlock(Model);
		}

		public static bool IsNull(BlockFullData instance)
		{
			if (instance == null) return true;
			return (instance.Model == null);
		}
		public Model.Block Model;
		public int[] ReportData;
		public bool CanAddSetupBefore;
	}
}