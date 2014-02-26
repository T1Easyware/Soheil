using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP
{
	public class BlockFullData
	{
		public static bool IsNull(BlockFullData instance) { if (instance == null)return true; return (instance.Model == null); }
		public Model.Block Model;
		public int[] ReportData;
		public bool CanAddSetupBefore;
	}
}