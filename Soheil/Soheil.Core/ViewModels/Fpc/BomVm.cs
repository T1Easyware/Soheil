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
	/// ViewModel for BOM inside a BomsVm
	/// </summary>
	public class BomVm : TreeItemVm
	{
		/// <summary>
		/// Gets the model for this BOM
		/// </summary>
		public Model.BOM Model { get; private set; }
		/// <summary>
		/// Gets Id for model of this StateStation
		/// </summary>
		public override int Id { get { return Model == null ? -1 : Model.Id; } }

		/// <summary>
		/// Gets or sets a bindable value that indicates IsDefault
		/// </summary>
		public bool IsDefault
		{
			get { return (bool)GetValue(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}
		public static readonly DependencyProperty IsDefaultProperty =
			DependencyProperty.Register("IsDefault", typeof(bool), typeof(BomVm),
			new PropertyMetadata(false, (d, e) =>
			{
				var vm = (BomVm)d;
				var val = (bool)e.NewValue;
				if (val && !vm._isInitializing)
					foreach (var item in vm.ContainerS.ContentsList.OfType<BomVm>())
					{
						if (item.Model != vm.Model)
							item.IsDefault = false;
					}

				vm.Model.IsDefault = val;
			}));

		/// <summary>
		/// Gets or sets a bindable value for Quantity
		/// </summary>
		public double Quantity
		{
			get { return (double)GetValue(QuantityProperty); }
			set { SetValue(QuantityProperty, value); }
		}
		public static readonly DependencyProperty QuantityProperty =
			DependencyProperty.Register("Quantity", typeof(double), typeof(BomVm),
			new UIPropertyMetadata(0d, (d, e) =>
			{
				((BomVm)d).Model.Quantity = (double)e.NewValue;
				StateVm.AnyPropertyChangedCallback(d, e);
			}));

		/// <summary>
		/// Gets or sets a bindable collection that indicates Units
		/// </summary>
		public ObservableCollection<UnitSetVm> Units { get { return _units; } }
		private ObservableCollection<UnitSetVm> _units = new ObservableCollection<UnitSetVm>();
		/// <summary>
		/// Gets or sets a bindable value that indicates Unit
		/// </summary>
		public UnitSetVm Unit
		{
			get { return (UnitSetVm)GetValue(UnitProperty); }
			set { SetValue(UnitProperty, value); }
		}
		public static readonly DependencyProperty UnitProperty =
			DependencyProperty.Register("Unit", typeof(UnitSetVm), typeof(BomVm),
			new PropertyMetadata(null, (d, e) =>
			{
				((BomVm)d).Model.UnitSet = ((UnitSetVm)e.NewValue).Model;
				StateVm.AnyPropertyChangedCallback(d, e);
			}));



		private bool _isInitializing = true;


		/// <summary>
		/// Creates a new instance of BomVm with given model and parent window
		/// </summary>
		/// <param name="parentWindowVm"></param>
		/// <param name="model">Can't be null</param>
		public BomVm(FpcWindowVm parentWindowVm, Model.BOM model)
			: base(parentWindowVm)
		{
			TreeLevel = 4;

			_isInitializing = true;

			var units = parentWindowVm.fpcDataService
				.rawMaterialDataService.GetUnitSets(model.RawMaterial)
				.Select(x => new UnitSetVm(x));
            //Units.Add(new UnitSetVm(null));
			foreach (var unit in units)
			{
				Units.Add(unit);
			}

			Model = model;
			Quantity = model.Quantity;
			IsDefault = model.IsDefault;
			if (model.UnitSet == null)
				Unit = Units.First();
			else
				Unit = Units.FirstOrDefault(x => x.Model.Id == model.UnitSet.Id);
			_isInitializing = false;
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
