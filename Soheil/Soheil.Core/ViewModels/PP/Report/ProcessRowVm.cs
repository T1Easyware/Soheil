using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class ProcessRowVm : StateStationActivityVm
	{
		/// <summary>
		/// Creates a ProcessRowVm to load process reports
		/// </summary>
		/// <param name="ssa"></param>
		public ProcessRowVm(Model.StateStationActivity model)
			: base(model)
		{
			//manual ProcessList.CollectionChanged += ProcessList_CollectionChanged;
		}

		void ProcessList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			RearrangeRows();
		}
		public void RearrangeRows()
		{
			if (!ProcessList.Any()) return;

			var tmp = new List<List<ProcessVm>>();
			foreach (var process in ProcessList.OrderBy(x => x.Model.StartDateTime))
			{
				int bestRow = -1;
				DateTime bestEndDt = DateTime.MinValue;
				for (int i = 0; i < tmp.Count; i++)
				{
					var dt = tmp[i].Last().Model.EndDateTime;
					if (dt <= process.Model.StartDateTime && dt > bestEndDt)
					{
						bestRow = i;
						bestEndDt = dt;
					}
				}

				if (bestRow == -1)
				{
					bestRow = tmp.Count;
					var row = new List<ProcessVm>();
					tmp.Add(row);
					row.Add(process);
					process.RowIndex = bestRow;
				}
				else
				{
					process.RowIndex = bestRow;
					tmp[bestRow].Add(process);
				}
			}
			RowsCount = tmp.Count;
		}

		public void RemoveProcessReport(ProcessVm vm)
		{
			ProcessList.RemoveWhere(x => x.Id == vm.Id);
		}

		//RowsCount Dependency Property
		public int RowsCount
		{
			get { return (int)GetValue(RowsCountProperty); }
			set { SetValue(RowsCountProperty, value); }
		}
		public static readonly DependencyProperty RowsCountProperty =
			DependencyProperty.Register("RowsCount", typeof(int), typeof(ProcessRowVm), new UIPropertyMetadata(0));

		/// <summary>
		/// Gets a bindable collection of process reports
		/// </summary>
		public ObservableCollection<ProcessVm> ProcessList { get { return _processList; } }
		private ObservableCollection<ProcessVm> _processList = new ObservableCollection<ProcessVm>();
	}
}
