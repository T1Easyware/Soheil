using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Soheil.Core.DataServices;

namespace Soheil.Core.ViewModels.Fpc
{
	public abstract class FpcVm : ViewModel
	{
		protected FPCDataService _fpcDataService;
		protected StationDataService _stationDataService;
		protected ActivityGroupDataService _activityGroupDataService;
		protected StateDataService _stateDataService;
		protected ConnectorDataService _connectorDataService;

		public Model.FPC Model { get; protected set; }

		public virtual void LoadAll(Model.FPC model)
		{
			Model = model;
			ProductReworks.Clear();
			if (model == null)
			{
				Id = -1;
				Product = null;
			}
			else
			{
				Id = model.Id;
				IsDefault = model.IsDefault;
				var productReworkModels = _fpcDataService.GetProductReworks(model, includeMainProduct: false);
				foreach (var prodrew in productReworkModels)
				{
					ProductReworks.Add(new ProductReworkVm(prodrew));
				}
				Product = new ProductVm(model.Product);
			}
		}

		//Product Dependency Property
		public ProductVm Product
		{
			get { return (ProductVm)GetValue(ProductProperty); }
			set { SetValue(ProductProperty, value); }
		}
		public static readonly DependencyProperty ProductProperty =
			DependencyProperty.Register("Product", typeof(ProductVm), typeof(FpcVm), new UIPropertyMetadata(null));
		//IsDefault Dependency Property
		public bool IsDefault
		{
			get { return (bool)GetValue(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}
		public static readonly DependencyProperty IsDefaultProperty =
			DependencyProperty.Register("IsDefault", typeof(bool), typeof(FpcVm),
			new UIPropertyMetadata(true, (d, e) => ((FpcVm)d).IsDefaultChanged((bool)e.NewValue)));
		public virtual void IsDefaultChanged(bool newValue) { /* will be handled thru DS of derived */ }
		//connectors Observable Collection
		private ObservableCollection<ConnectorVm> _connectors = new ObservableCollection<ConnectorVm>();
		public ObservableCollection<ConnectorVm> Connectors { get { return _connectors; } }
		//states Observable Collection
		private ObservableCollection<StateVm> _states = new ObservableCollection<StateVm>();
		public ObservableCollection<StateVm> States { get { return _states; } }
		//ProductReworks Observable Collection
		private ObservableCollection<ProductReworkVm> _productReworks = new ObservableCollection<ProductReworkVm>();
		public ObservableCollection<ProductReworkVm> ProductReworks { get { return _productReworks; } }
	}
}
