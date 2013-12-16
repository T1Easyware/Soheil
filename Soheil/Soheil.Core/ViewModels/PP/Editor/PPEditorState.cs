using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PPEditorState : DependencyObject
	{
		#region Ctor and Methods
		/// <summary>
		/// Must be called within an EdmContext
		/// </summary>
		/// <param name="model"></param>
		public PPEditorState(Model.Task model)
		{
			StateId = model.StateStation.State.Id;
			Name = model.StateStation.State.Name;
			Code = model.StateStation.State.Code;
			Product = new ViewModels.PP.ProductVm(model.StateStation.State.FPC.Product, null);
			StationList.Add(new PPEditorStation(this, model));
		}
		public PPEditorState(Fpc.StateVm fpcState)
		{
			StateId = fpcState.Id;
			Name = fpcState.Name;
			Code = fpcState.Code;
			Product = new ProductVm(fpcState.FPC.Product);
			//if (fpcState.Config == null) new DataServices.StateDataService().FetchConfig(fpcState);
			foreach (Fpc.StateStationVm ss in fpcState.Config.ContentsList)
			{
				StationList.Add(new PPEditorStation(this, ss));
			}
		}
		public void ResetCurrentStation()
		{
			StationList[FocusedStationTabIndex].Reset();
		}
		#endregion

		public int StateId { get; set; }

		#region DpProps
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(PPEditorState), new UIPropertyMetadata(null));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(PPEditorState), new UIPropertyMetadata(null));
		//Product Dependency Property
		public ProductVm Product
		{
			get { return (ProductVm)GetValue(ProductProperty); }
			set { SetValue(ProductProperty, value); }
		}
		public static readonly DependencyProperty ProductProperty =
			DependencyProperty.Register("Product", typeof(ProductVm), typeof(PPEditorState), new UIPropertyMetadata(null));
		//StationList Observable Collection
		private ObservableCollection<PPEditorStation> _stationList = new ObservableCollection<PPEditorStation>();
		public ObservableCollection<PPEditorStation> StationList { get { return _stationList; } }
		//FocusedStationTabIndex Dependency Property
		public int FocusedStationTabIndex
		{
			get { return (int)GetValue(FocusedStationTabIndexProperty); }
			set { SetValue(FocusedStationTabIndexProperty, value); }
		}
		public static readonly DependencyProperty FocusedStationTabIndexProperty =
			DependencyProperty.Register("FocusedStationTabIndex", typeof(int), typeof(PPEditorState), new UIPropertyMetadata(0));
		//HasUnsavedChanges Dependency Property
		public bool HasUnsavedChanges
		{
			get { return (bool)GetValue(HasUnsavedChangesProperty); }
			set { SetValue(HasUnsavedChangesProperty, value); }
		}
		public static readonly DependencyProperty HasUnsavedChangesProperty =
			DependencyProperty.Register("HasUnsavedChanges", typeof(bool), typeof(PPEditorState), new UIPropertyMetadata(false));
		#endregion
	}
}
