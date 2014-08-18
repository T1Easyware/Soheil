using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Soheil.Common;

namespace Soheil.Core.ViewModels.MessageCenter
{
	public class NotificationArea : DependencyObject
	{
		/// <summary>
		/// Gets or sets the only instance of this class
		/// </summary>
		public static NotificationArea Singleton { get; set; }
		public event Action Loaded;
		/// <summary>
		/// Instantiate and load information asyncly
		/// </summary>
		public NotificationArea()
		{

		}
		public void Load()
		{
			//Task.Run(() =>
			//{
			//read data
			var pmList = new DataServices.PM.MaintenanceDataService().GetAlarms().ToArray()//ToArray may be needed to execute the query and to update CalculatedDiffDays
				.OrderBy(x => x.CalculatedDiffDays);
			var repairList = new DataServices.PM.RepairDataService().GetAlarms().ToArray();

			//count
			int count = pmList.Count(x => x.CalculatedDiffDays <= 0d)
				+ repairList.Count(x => x.RepairStatus == (byte)RepairStatus.Reported);

			var pmTop10 = pmList.Take(10);
			var repairTop10 = repairList.Take(10);

			//create vm
			//Dispatcher.Invoke(() =>
			//{
			if (pmTop10.Any())
			{
				List.Add(new NotificationVm { IsSeparator = true, Message = "PM" });
				foreach (var item in pmTop10)
				{
					List.Add(new PMVm(item));
				}
			}
			if (repairTop10.Any())
			{
				List.Add(new NotificationVm { IsSeparator = true, Message = "تعمیرات" });
				foreach (var item in repairTop10)
				{
					List.Add(new RepairVm(item));
				}
			}
			Count = count;
			//});
			//});
			if (Loaded != null) Loaded();
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates Count
		/// </summary>
		public int Count
		{
			get { return (int)GetValue(CountProperty); }
			set { SetValue(CountProperty, value); }
		}
		public static readonly DependencyProperty CountProperty =
			DependencyProperty.Register("Count", typeof(int), typeof(NotificationArea), new PropertyMetadata(-1));

		/// <summary>
		/// Gets or sets a bindable collection that indicates List
		/// </summary>
		public ObservableCollection<NotificationVm> List { get { return _list; } }
		private ObservableCollection<NotificationVm> _list = new ObservableCollection<NotificationVm>();



	}
}
