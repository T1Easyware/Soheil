using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// A specialized treeItem to be used as a temporary hollow (sub)item in a state
	/// <para>Id of this treeItem is 0</para>
	/// </summary>
	public class DropIndicatorVm : TreeItemVm
	{
		/// <summary>
		/// Creates an instance of this view model with given parameteres
		/// </summary>
		/// <param name="parent">parent FpcWindowVm instance</param>
		/// <param name="container">(sub)item in fpc state in which this vm desires to go</param>
		/// <param name="containment">toolbox item that contains the data which is being dragged onto the state</param>
		public DropIndicatorVm(FpcWindowVm parent, TreeItemVm container, IToolboxData containment)
			: base(parent)
		{
			Container = container;
			Containment = containment;
			IsDropIndicator = true;
		}

		/// <summary>
		/// Gets the overrided Id of this TreeItem which is equal to 0
		/// </summary>
		public override int Id
		{
			get { return 0; }
		}
	}
}
