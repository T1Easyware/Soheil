using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.SetupTime
{
	public enum CellType
	{
		None,
		TextCell,

		ProductGroupColumnHeaderCell,
		ProductGroupRowHeaderCell,
	
		ProductColumnHeaderCell,
		ProductRowHeaderCell,
		
		ReworkColumnHeaderCell,
		ReworkRowHeaderCell,
		
		CheckBoxCell,
		WarmupCell,
		ChangeoverCell
	}
}
