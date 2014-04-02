using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP
{
	public class BlockFullData
	{
		private DataServices.BlockDataService BlockDataService;
		public BlockFullData(DataServices.BlockDataService blockDataService, int blockId)
		{
			this.BlockDataService = blockDataService;
			Model = BlockDataService.GetSingleFull(blockId);
			ReportData = BlockDataService.GetProductionReportData(Model);
			CanAddSetupBefore = BlockDataService.CanAddSetupBeforeBlock(Model);
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