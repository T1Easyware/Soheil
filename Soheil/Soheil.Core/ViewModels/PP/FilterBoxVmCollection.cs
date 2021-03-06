﻿using System;
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
		/// Occurs when Operator is selected in one of filterBoxes (sender, oldValue, newValue)
		/// </summary>
		public event Action<FilterBoxVm, FilterableItemVm, FilterableItemVm> OperatorSelected;
		/// <summary>
		/// Occurs when Operator (filterBox) is removed
		/// </summary>
		public event Action<FilterBoxVm> OperatorRemoved;

		/// <summary>
		/// Creates an instance of FilterBoxVmCollection to be used as a collection of guilty operators
		/// </summary>
		/// <param name="models">collection of ODR or OSR models which represent guilty operators</param>
		/// <returns></returns>
		public static FilterBoxVmCollection CreateForGuiltyOperators(dynamic models, Dal.SoheilEdmContext uow)
		{
			//find all active operators
			var operatorDs = new DataServices.OperatorDataService(uow);
			var operatorModels = operatorDs.GetActives();
			
			//create vm for all active operators
			var operatorVms = new FilterableItemVm[operatorModels.Count];
			for (int i = 0; i < operatorVms.Length; i++)
			{
				operatorVms[i] = FilterableItemVm.CreateForGuiltyOperator(operatorModels[i]);
			}

			//initiate this vm to auto-add operatorVms when a new FilterBoxVm is added to it
			var vm = new FilterBoxVmCollection();
			vm.AddCommand = new Commands.Command(o => vm.AddOperator(operatorVms));

			//add new FilterBoxVm for each of guilty operators and select the guilty operator in it
			if (models != null)
			{
				foreach (var model in models)
				{
					vm.AddOperator(operatorVms, model);
				}
			}

			return vm;
		}

		/// <summary>
		/// Adds a FilterBoxVm to this collection specialized for guilty operators
		/// </summary>
		/// <param name="operatorVms">ViewModels for operators to be added to the filterBox</param>
		/// <param name="model">Model of an existing ODR or OSR model used to create this FilterBox</param>
		private void AddOperator(FilterableItemVm[] operatorVms, object model = null)
		{
			int operId = 0;
			if (model is Model.OperatorDefectionReport)
			{
				var odr = (model as Model.OperatorDefectionReport);
				operId = odr.Operator.Id;
			}
			else if (model is Model.OperatorStoppageReport)
			{
				var osr = (model as Model.OperatorStoppageReport);
				operId = osr.Operator.Id;
			}


			var fb = FilterBoxVm.CreateForGuiltyOperators(this, operId, operatorVms);
			fb.Model = model;


			fb.FilterableItemSelected += (vm, oldOp, newOp) =>
			{
				if (newOp == null) return;
				if (newOp.Model != null)
				{
					if (OperatorSelected != null)
						OperatorSelected(vm, oldOp, newOp);
				}
			};
			fb.FilterBoxDeleted += vm =>
			{
				if (OperatorRemoved != null)
					OperatorRemoved(vm);
			};

			if (fb.SelectedItem == null) fb.SelectedItem = fb.FilteredList.FirstOrDefault();

			FilterBoxes.Add(fb);
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
			var causeL1Models = causeDs.GetRoot().Children.Where(x => x.Status == (byte)Common.Status.Active).ToArray();
			var causeL3Box = FilterBoxVm.CreateForCause(null);
			var causeL2Box = FilterBoxVm.CreateForCause(causeL3Box);
			var causeL1Box = FilterBoxVm.CreateForCause(causeL2Box, causeL1Models);

			//set the event handlers
			causeL3Box.FilterableItemSelected += (s, old, v) =>
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
				parent.SelectCause(v.Id);
			};
			causeL2Box.FilterableItemSelected += (s, old, v) =>
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
				foreach (var item in ((CauseVm)v.ViewModel).ChildrenModels.Where(x => x.Status == (byte)Common.Status.Active))
				{
					causeL3Box.FilteredList.Add(FilterableItemVm.CreateForCause(causeL3Box, item));
				}
			};
			causeL1Box.FilterableItemSelected += (s, old, v) =>
			{
				if (v != null)
					parent.SelectedCode = ((CauseVm)v.ViewModel).Code;

				causeL2Box.FilteredList.Clear();
				if (v == null) return;
				foreach (var item in ((CauseVm)v.ViewModel).ChildrenModels.Where(x => x.Status == (byte)Common.Status.Active))
				{
					causeL2Box.FilteredList.Add(FilterableItemVm.CreateForCause(causeL2Box, item));
				}
			};

			//add filterboxes to the collection
			vm.FilterBoxes.Add(causeL1Box);
			vm.FilterBoxes.Add(causeL2Box);
			vm.FilterBoxes.Add(causeL3Box);

			//select the default cause
			try
			{
				if (selectedIds != null)
				{

					causeL1Box.SelectedItem = causeL1Box.FilteredList.FirstOrDefault(x => x.Id == selectedIds[0]);
					causeL2Box.SelectedItem = causeL2Box.FilteredList.FirstOrDefault(x => x.Id == selectedIds[1]);
					causeL3Box.SelectedItem = causeL3Box.FilteredList.FirstOrDefault(x => x.Id == selectedIds[2]);
				}
				else
				{
					causeL1Box.SelectedItem = causeL1Box.FilteredList.FirstOrDefault();
					causeL2Box.SelectedItem = causeL2Box.FilteredList.FirstOrDefault();
					causeL3Box.SelectedItem = causeL3Box.FilteredList.FirstOrDefault();
				}
			}
			catch { }

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
