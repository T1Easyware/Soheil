using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class FilterBoxVmCollection : DependencyObject
	{
		public static FilterBoxVmCollection CreateForGuiltyOperators(IEnumerable<int> operatorIds)
		{
			var operatorDs = new DataServices.OperatorDataService();
			var operatorModels = operatorDs.GetActives();
			var operatorVms = new FilterableItemVm[operatorModels.Count];
			for (int i = 0; i < operatorVms.Length; i++)
			{
				operatorVms[i] = FilterableItemVm.CreateForGuiltyOperator(operatorModels[i]);
			}
			var vm = new FilterBoxVmCollection();
			vm.AddCommand = new Commands.Command(o => vm.AddOperator(operatorVms, -1));

			//select insert
			if (operatorIds != null)
			{
				foreach (var operatorId in operatorIds)
				{
					vm.AddOperator(operatorVms, operatorId);
				}
			}

			return vm;
		}
		private void AddOperator(FilterableItemVm[] operatorVms, int selectedId)
		{
			FilterBoxes.Add(FilterBoxVm.CreateForGuiltyOperators(this, selectedId, operatorVms));
		}

		public static FilterBoxVmCollection CreateForStoppageReport(StoppageReportVm parent, int[] selectedIds)
		{
			var vm = new FilterBoxVmCollection();

			var causeDs = new DataServices.CauseDataService();
			var causeL1Models = causeDs.GetActives().Where(x => x.Level == 0).ToArray();
			var causeL3Box = FilterBoxVm.CreateForCause(null);
			var causeL2Box = FilterBoxVm.CreateForCause(causeL3Box);
			var causeL1Box = FilterBoxVm.CreateForCause(causeL2Box, causeL1Models);
			causeL3Box.FilterBoxSelectedItemChanged += (s, v) =>
			{
				string code = string.Empty;
				if (causeL1Box.SelectedItem != null)
				{
					code = ((CauseVm)causeL1Box.SelectedItem.ViewModel).Code;
					if (causeL2Box.SelectedItem != null)
					{
						code += ((CauseVm)causeL2Box.SelectedItem.ViewModel).Code;
						if (v != null) 
							code += ((CauseVm)v.ViewModel).Code;
					}
				}
				parent.SelectedCode =  code;
			};
			causeL2Box.FilterBoxSelectedItemChanged += (s, v) =>
			{
				string code = string.Empty;
				if (causeL1Box.SelectedItem != null)
				{
					code = ((CauseVm)causeL1Box.SelectedItem.ViewModel).Code;
					if (v != null)
						code += ((CauseVm)v.ViewModel).Code;
				}
				parent.SelectedCode = code;

				causeL3Box.FilteredList.Clear();
				if (v == null) return;
				foreach (var item in ((CauseVm)v.ViewModel).ChildrenModels)
				{
					causeL3Box.FilteredList.Add(FilterableItemVm.CreateForCause(causeL3Box, item));
				}
			};
			causeL1Box.FilterBoxSelectedItemChanged += (s, v) =>
			{
				if (v != null)
					parent.SelectedCode = ((CauseVm)v.ViewModel).Code;

				causeL2Box.FilteredList.Clear();
				if (v == null) return;
				foreach (var item in ((CauseVm)v.ViewModel).ChildrenModels)
				{
					causeL2Box.FilteredList.Add(FilterableItemVm.CreateForCause(causeL2Box, item));
				}
			};

			vm.FilterBoxes.Add(causeL1Box);
			vm.FilterBoxes.Add(causeL2Box);
			vm.FilterBoxes.Add(causeL3Box);

			//select
			if (selectedIds != null)
			{
				causeL1Box.SelectedItem = causeL1Box.FilteredList.FirstOrDefault(x => x.Id == selectedIds[0]);
				causeL2Box.SelectedItem = causeL2Box.FilteredList.FirstOrDefault(x => x.Id == selectedIds[1]);
				causeL3Box.SelectedItem = causeL3Box.FilteredList.FirstOrDefault(x => x.Id == selectedIds[2]);
			}

			vm.AddCommand = new Commands.Command(o => { });
			return vm;
		}
		

		//FilterBoxes Observable Collection
		private ObservableCollection<FilterBoxVm> _filterBoxes = new ObservableCollection<FilterBoxVm>();
		public ObservableCollection<FilterBoxVm> FilterBoxes { get { return _filterBoxes; } }
		//AddCommand Dependency Property
		public Commands.Command AddCommand
		{
			get { return (Commands.Command)GetValue(AddCommandProperty); }
			set { SetValue(AddCommandProperty, value); }
		}
		public static readonly DependencyProperty AddCommandProperty =
			DependencyProperty.Register("AddCommand", typeof(Commands.Command), typeof(FilterBoxVmCollection), new UIPropertyMetadata(null));
	}
}
