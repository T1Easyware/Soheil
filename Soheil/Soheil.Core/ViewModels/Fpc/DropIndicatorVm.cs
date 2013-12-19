using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.Fpc
{
	public class DropIndicatorVm : TreeItemVm
	{
		public DropIndicatorVm(FpcWindowVm parent, TreeItemVm container, IToolboxData containment)
			: base(parent)
		{
			Container = container;
			Containment = containment;
			IsDropIndicator = true;
		}

		public override int Id
		{
			get { return 0; }
		}
	}
}
