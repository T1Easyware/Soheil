using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class StateVm : DependencyObject
	{
		public StateVm(Model.State model)
		{
			Id = model.Id;
			Name = model.Name;
			Code = model.Code;
			ProductRework = new ProductReworkVm(model.OnProductRework);
			foreach (var ss in model.StateStations)
			{
				StateStationList.Add(new StateStationVm(ss));
			}
		}
		public int Id { get; private set; }
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(StateVm), new UIPropertyMetadata(null));
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(StateVm), new UIPropertyMetadata(null));

		//ProductRework Dependency Property
		public ProductReworkVm ProductRework
		{
			get { return (ProductReworkVm)GetValue(ProductReworkProperty); }
			set { SetValue(ProductReworkProperty, value); }
		}
		public static readonly DependencyProperty ProductReworkProperty =
			DependencyProperty.Register("ProductRework", typeof(ProductReworkVm), typeof(StateVm), new UIPropertyMetadata(null));

		//StateStationList Observable Collection
		private ObservableCollection<StateStationVm> _stateStationList = new ObservableCollection<StateStationVm>();
		public ObservableCollection<StateStationVm> StateStationList { get { return _stateStationList; } }
	}
}
