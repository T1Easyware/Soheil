using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.MaterialPlanning
{
	public class CellVm : DependencyObject
	{
		/// <summary>
		/// Gets or sets a bindable collection that indicates Requests
		/// </summary>
		public ObservableCollection<RequestVm> Requests { get { return _requests; } }
		private ObservableCollection<RequestVm> _requests = new ObservableCollection<RequestVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates RawMaterial
		/// </summary>
		public RawMaterialVm RawMaterial
		{
			get { return (RawMaterialVm)GetValue(RawMaterialProperty); }
			set { SetValue(RawMaterialProperty, value); }
		}
		public static readonly DependencyProperty RawMaterialProperty =
			DependencyProperty.Register("RawMaterial", typeof(RawMaterialVm), typeof(CellVm), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value that indicates Hour
		/// </summary>
		public HourVm Hour
		{
			get { return (HourVm)GetValue(HourProperty); }
			set { SetValue(HourProperty, value); }
		}
		public static readonly DependencyProperty HourProperty =
			DependencyProperty.Register("Hour", typeof(HourVm), typeof(CellVm), new PropertyMetadata(null));
	}
}
