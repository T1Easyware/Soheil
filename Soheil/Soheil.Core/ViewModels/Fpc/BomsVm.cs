using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// ViewModel for all BOMs inside a State
	/// </summary>
	public class BomsVm : TreeItemVm
	{

		public override int Id
		{
			get { throw new NotImplementedException(); }
		}
		/// <summary>
		/// Creates a new instance of BomVm with given model and parent window
		/// </summary>
		/// <param name="parentWindowVm"></param>
		/// <param name="model">Can't be null</param>
		public BomsVm(FpcWindowVm parentWindowVm, Model.BOM model)
			: base(parentWindowVm)
		{
			TreeLevel = 4;
		}

		/// <summary>
		/// Gets or sets Container of this StateStation (cast to StateConfigVm)
		/// </summary>
		public StateConfigVm ContainerS { get { return (StateConfigVm)base.Container; } set { base.Container = value; } }
		/// <summary>
		/// Gets or sets Containment of this BOM (cast to RawMaterialVm)
		/// </summary>
		public RawMaterialVm ContainmentRawMaterial { get { return (RawMaterialVm)base.Containment; } set { base.Containment = value; } }

		/// <summary>
		/// If called with newValue = true, Collapses siblings BOMs of this
		/// And also sets focus to this State and selects this State
		/// </summary>
		/// <param name="newValue"></param>
		protected override void isExpandedChanged(bool newValue)
		{
			if (newValue)
			{
				//collapse other StateStationActivityMachines in parent StateStationActivity of this
				var q = Container.ContentsList.Where(x => x.IsExpanded && x != this);
				foreach (var item in q) item.IsExpanded = false;
				//set focus to parent State
				//Parent.FocusedState = ContainerS.State;
				//select parent StateStation
				//Parent.OnStationSelected(Parent.FocusedStateStation);
			} 
		}


		/// <summary>
		/// Removes this BomVm from State
		/// </summary>
		public override void Delete()
		{
			//delete
			Container.ContentsList.Remove(this);
		}

	}
}
