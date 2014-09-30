using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Core.DataServices;
using Soheil.Common;
using Soheil.Core.Interfaces;
using System.Collections.ObjectModel;
using Soheil.Core.ViewModels.Fpc.ListItems;

namespace Soheil.Core.ViewModels.Fpc
{
	public class FpcManagerVm : DependencyObject, ISingularList
	{
		#region Properties and Events
		/// <summary>
		/// Gets or sets the data service.
		/// </summary>
		public ProductGroupDataService ProductGroupDataService { get; set; }
		public AccessType Access { get; private set; }
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;


		/// <summary>
		/// Gets or sets a bindable collection of ProductGroups
		/// </summary>
		public ObservableCollection<ProductGroupVm> ProductGroups { get { return _productGroups; } }
		private ObservableCollection<ProductGroupVm> _productGroups = new ObservableCollection<ProductGroupVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates SelectedFpc
		/// </summary>
		public FpcWindowVm SelectedFpc
		{
			get { return (FpcWindowVm)GetValue(SelectedFpcProperty); }
			set { SetValue(SelectedFpcProperty, value); }
		}
		public static readonly DependencyProperty SelectedFpcProperty =
			DependencyProperty.Register("SelectedFpc", typeof(FpcWindowVm), typeof(FpcManagerVm), new UIPropertyMetadata(null));
		#endregion

		#region Ctor and Init
		public FpcManagerVm(AccessType access)
		{
			Access = access;
			Refresh();
			AutoSelectLastFpc();
			RefreshCommand = new Commands.Command(o => {
				var id = 0;
				if (SelectedFpc != null)
					id = SelectedFpc.Id;
				Refresh();
				foreach (var pg in ProductGroups)
				{
					foreach (var p in pg.Products)
					{
						if (p.Fpcs.Any(x => x.Id == id))
						{
							p.SelectedFpc = p.Fpcs.First(x => x.Id == id);
							p.IsExpanded = true;
							pg.IsExpanded = true;
							return;
						}
					}
				}

			});
		}
		#endregion

		#region Methods
		private void AutoSelectLastFpc()
		{
			//auto select last used fpc
			int id = Properties.Settings.Default.LastFpcId;
			if (id > 0)
			{
				foreach (var pg in ProductGroups)
				{
					foreach (var p in pg.Products)
					{
						foreach (var fpc in p.Fpcs)
						{
							if (fpc.Id == id)
							{
								p.SelectedFpc = fpc;
								p.IsExpanded = true;
								pg.IsExpanded = true;
								return;
							}
						}
					}
				}
			}
		}

		public void Refresh()
		{
			ProductGroups.Clear();
			
			var uow = new Dal.SoheilEdmContext();
			ProductGroupDataService = new ProductGroupDataService(uow);
			var list = ProductGroupDataService.GetActives();
			//reload all ProductGroups
			foreach (var item in list)
			{
				var pgVm = new ProductGroupVm(item);
				pgVm.SelectionChanged += pgVm_SelectionChanged;
				pgVm.FpcAddedToProduct += pgVm_FpcAddedToProduct;
				ProductGroups.Add(pgVm);
			}
		}

		void pgVm_FpcAddedToProduct(ListItems.ProductVm product)
		{
			Model.FPC newModel = new Model.FPC
			{
				Name = "*",
				Code = "*",
				CreatedDate = DateTime.Now,
				ModifiedDate = DateTime.Now
			};
			var ds = new DataServices.FPCDataService();
			ds.AddModel(newModel, product.Id);

			Refresh();
			foreach (var pg in ProductGroups)
			{
				foreach (var p in pg.Products)
				{
					if (p.Fpcs.Any(x => x.Id == newModel.Id))
					{
						p.SelectedFpc = p.Fpcs.First(x => x.Id == newModel.Id);
						p.IsExpanded = true;
						pg.IsExpanded = true;
						return;
					}
				}
			}

		}

		void pgVm_SelectionChanged(ListItems.FpcVm fpc)
		{
			//save last used fpc
			if (fpc != null)
			{
				Properties.Settings.Default.LastFpcId = fpc.Id;
				Properties.Settings.Default.Save();
			}
			foreach (var pg in ProductGroups)
			{
				foreach (var p in pg.Products)
				{
					if (!p.Fpcs.Contains(fpc))
						p.SelectedFpc = null;
				}
			}
			if (SelectedFpc == null)
			{
				SelectedFpc = new FpcWindowVm();
				SelectedFpc.DefaultChanged += SelectedFpc_DefaultChanged;
				SelectedFpc.Duplicated += SelectedFpc_Duplicated;
			}
			if (fpc != null)
				SelectedFpc.ChangeFpcByFpcId(fpc.Id, Access);
		}

		void SelectedFpc_Duplicated(Model.FPC newModel)
		{
			Refresh();
			foreach (var pg in ProductGroups)
			{
				foreach (var p in pg.Products)
				{
					if (p.Fpcs.Any(x => x.Id == newModel.Id))
					{
						p.SelectedFpc = p.Fpcs.First(x => x.Id == newModel.Id);
						p.IsExpanded = true;
						pg.IsExpanded = true;
						return;
					}
				}
			}
		}

		void SelectedFpc_DefaultChanged(int id)
		{
			foreach (var pg in ProductGroups)
			{
				foreach (var p in pg.Products)
				{
					if (p.Fpcs.Any(x => x.Id == id))
					{
						foreach (var fpc in p.Fpcs)
						{
							fpc.IsDefault = fpc.Id == id;
						}
						return;
					}
				}
			}
		}
		#endregion

		#region Commands
		/// <summary>
		/// Gets or sets a bindable value that indicates RefreshCommand
		/// </summary>
		public Commands.Command RefreshCommand
		{
			get { return (Commands.Command)GetValue(RefreshCommandProperty); }
			set { SetValue(RefreshCommandProperty, value); }
		}
		public static readonly DependencyProperty RefreshCommandProperty =
			DependencyProperty.Register("RefreshCommand", typeof(Commands.Command), typeof(FpcManagerVm), new UIPropertyMetadata(null));
		#endregion

	}
}
