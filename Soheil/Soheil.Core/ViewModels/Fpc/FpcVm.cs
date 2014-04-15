using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Soheil.Core.DataServices;
using Soheil.Core.Base;
using Soheil.Common;

namespace Soheil.Core.ViewModels.Fpc
{
	public abstract class FpcVm : ViewModelBase
	{
		public FPCDataService fpcDataService { get; protected set; }

		public Model.FPC Model { get; protected set; }

		public int Id { get { return Model == null ? -1 : Model.Id; } }

		public FpcVm()
		{
			States.CollectionChanged += (s, e) => HasStates = States.Any();
		}

		#region Methods
		/// <summary>
		/// Loads PRs and sets Model basic props
		/// </summary>
		/// <param name="model"></param>
		protected void initByModel(Model.FPC model)
		{
			Model = model;
			ProductReworks.Clear();
			if (model == null)
			{
				Product = null;
			}
			else
			{
				IsDefault = model.IsDefault;
				var productReworkModels = fpcDataService.GetProductReworks(model, includeMainProduct: false);
				foreach (var prodrew in productReworkModels)
				{
					ProductReworks.Add(new ProductReworkVm(prodrew));
				}
				Product = new ProductVm(model.Product);
			}
		}
		public virtual void IsDefaultChanged(bool newValue) { /* will be handled thru DS of derived */ }
		#endregion

		#region Props
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

		//connectors Observable Collection
		public ObservableCollection<ConnectorVm> Connectors { get { return _connectors; } }
		private ObservableCollection<ConnectorVm> _connectors = new ObservableCollection<ConnectorVm>();
		//states Observable Collection
		public ObservableCollection<StateVm> States { get { return _states; } }
		private ObservableCollection<StateVm> _states = new ObservableCollection<StateVm>();
		//ProductReworks Observable Collection
		public ObservableCollection<ProductReworkVm> ProductReworks { get { return _productReworks; } }
		private ObservableCollection<ProductReworkVm> _productReworks = new ObservableCollection<ProductReworkVm>();
		#endregion

		protected void initCommands()
		{
			ExpandAllCommand = new Commands.Command(o =>
			{
				var items = States.Where(x => x.StateType == StateType.Mid);
				foreach (var item in items)
				{
					item.ShowDetails = true;
				}
			});
			CollapseAllCommand = new Commands.Command(o =>
			{
				foreach (var item in States.Where(x => x.StateType == StateType.Mid))
				{
					item.ShowDetails = false;
				}
			});
		}
		//ExpandAllCommand Dependency Property
		public Commands.Command ExpandAllCommand
		{
			get { return (Commands.Command)GetValue(ExpandAllCommandProperty); }
			set { SetValue(ExpandAllCommandProperty, value); }
		}
		public static readonly DependencyProperty ExpandAllCommandProperty =
			DependencyProperty.Register("ExpandAllCommand", typeof(Commands.Command), typeof(FpcVm), new UIPropertyMetadata(null));
		//CollapseAllCommand Dependency Property
		public Commands.Command CollapseAllCommand
		{
			get { return (Commands.Command)GetValue(CollapseAllCommandProperty); }
			set { SetValue(CollapseAllCommandProperty, value); }
		}
		public static readonly DependencyProperty CollapseAllCommandProperty =
			DependencyProperty.Register("CollapseAllCommand", typeof(Commands.Command), typeof(FpcVm), new UIPropertyMetadata(null));

		//HasStates Dependency Property
		public bool HasStates
		{
			get { return (bool)GetValue(HasStatesProperty); }
			set { SetValue(HasStatesProperty, value); }
		}
		public static readonly DependencyProperty HasStatesProperty =
			DependencyProperty.Register("HasStates", typeof(bool), typeof(FpcVm), new UIPropertyMetadata(false));
	}
}
