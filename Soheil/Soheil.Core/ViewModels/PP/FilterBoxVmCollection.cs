using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	/// <summary>
	/// ViewModel for a collection of <see cref="FilterBoxVm"/> instances
	/// <para>Used for guilty operators and stoppage report (cause)</para>
	/// </summary>
	public class FilterBoxVmCollection : DependencyObject
	{
		/// <summary>
		/// Creates an instance of FilterBoxVmCollection to be used as a collection of guilty operators
		/// </summary>
		/// <param name="operatorIds">collection of operator Ids that are guilty by default</param>
		/// <returns></returns>
		public static FilterBoxVmCollection CreateForGuiltyOperators(IEnumerable<int> operatorIds)
		{
			//find all active operators
			var operatorDs = new DataServices.OperatorDataService();
			var operatorModels = operatorDs.GetActives();
			
			//create vm for all active operators
			var operatorVms = new FilterableItemVm[operatorModels.Count];
			for (int i = 0; i < operatorVms.Length; i++)
			{
				operatorVms[i] = FilterableItemVm.CreateForGuiltyOperator(operatorModels[i]);
			}

			//initiate this vm to auto-add operatorVms when a new FilterBoxVm is added to it
			var vm = new FilterBoxVmCollection();
			vm.AddCommand = new Commands.Command(o => vm.AddOperator(operatorVms, -1));

			//add new FilterBoxVm for each of guilty operators and select the guilty operator in it
			if (operatorIds != null)
			{
				foreach (var operatorId in operatorIds)
				{
					vm.AddOperator(operatorVms, operatorId);
				}
			}

			return vm;
		}

		/// <summary>
		/// Adds a FilterBoxVm to this collection specialized for guilty operators
		/// </summary>
		/// <param name="operatorVms">ViewModels for operators to be added to the filterBox</param>
		/// <param name="selectedId">default guilty operator's Id</param>
		private void AddOperator(FilterableItemVm[] operatorVms, int selectedId)
		{
			FilterBoxes.Add(FilterBoxVm.CreateForGuiltyOperators(this, selectedId, operatorVms));
		}
		/// <summary>
		/// Creates an instance of FilterBoxVmCollection to be used as a collection of stoppage reports (cause)
		/// </summary>
		/// <param name="parent">Instance of <see cref="StoppageReportVm"/> that has this collection</param>
		/// <param name="selectedIds">default cause Ids (must be like: {level1Id, level2Id, level3Id})</param>
		/// <returns></returns>
		public static FilterBoxVmCollection CreateForStoppageReport(Report.StoppageReportVm parent, int[] selectedIds)
		{
			var vm = new FilterBoxVmCollection();

			//find causes and create FilterBoxVm instances for each level
			var causeDs = new DataServices.CauseDataService();
			var causeL1Models = causeDs.GetRoot().Children.ToArray();
			var causeL3Box = FilterBoxVm.CreateForCause(null);
			var causeL2Box = FilterBoxVm.CreateForCause(causeL3Box);
			var causeL1Box = FilterBoxVm.CreateForCause(causeL2Box, causeL1Models);

			//set the event handlers
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

			//add filterboxes to the collection
			vm.FilterBoxes.Add(causeL1Box);
			vm.FilterBoxes.Add(causeL2Box);
			vm.FilterBoxes.Add(causeL3Box);

			//select the default cause
			if (selectedIds != null)
			{
				try
				{
					causeL1Box.SelectedItem = causeL1Box.FilteredList.FirstOrDefault(x => x.Id == selectedIds[0]);
					causeL2Box.SelectedItem = causeL2Box.FilteredList.FirstOrDefault(x => x.Id == selectedIds[1]);
					causeL3Box.SelectedItem = causeL3Box.FilteredList.FirstOrDefault(x => x.Id == selectedIds[2]);
				}
				catch { }
			}

			vm.AddCommand = new Commands.Command(o => { });
			return vm;
		}
		

		/// <summary>
		/// Gets a bindable collection of <see cref="FilterBoxVm"/>s
		/// </summary>
		public ObservableCollection<FilterBoxVm> FilterBoxes { get { return _filterBoxes; } }
		private ObservableCollection<FilterBoxVm> _filterBoxes = new ObservableCollection<FilterBoxVm>();
		
		/// <summary>
		/// Gets a bindable command to add a new FilterBox to collection
		/// </summary>
		public Commands.Command AddCommand
		{
			get { return (Commands.Command)GetValue(AddCommandProperty); }
			protected set { SetValue(AddCommandProperty, value); }
		}
		public static readonly DependencyProperty AddCommandProperty =
			DependencyProperty.Register("AddCommand", typeof(Commands.Command), typeof(FilterBoxVmCollection), new UIPropertyMetadata(null));
	}
}
