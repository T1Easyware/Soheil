using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class OperatorManagerVm : DependencyObject
	{
		public event Action<OperatorEditorVm, bool, Soheil.Common.OperatorRole> SelectionChanged;

		Dal.SoheilEdmContext _uow;
		DataServices.OperatorDataService _operatorDataService;

		public OperatorManagerVm(Dal.SoheilEdmContext uow)
		{
			_uow = uow;

			//get all operators and convert them to VM
			_operatorDataService = new DataServices.OperatorDataService(_uow);
			var allOperators = _operatorDataService.GetActives();
			foreach (var oper in allOperators)
			{
				var operVm = new OperatorEditorVm(oper);
				operVm.SelectedOperatorsChanged += (isSelected, role) =>
				{
					if (SelectionChanged != null) SelectionChanged(operVm, isSelected, role);
				};
				OperatorsList.Add(operVm);
			}

			//init commands
			ClearSearchCommand = new Commands.Command(textBox =>
			{
				if (textBox is System.Windows.Controls.TextBox)
					(textBox as System.Windows.Controls.TextBox).Clear();
			});
			RefreshCommand = new Commands.Command(o => Refresh());
		}
		public void Refresh()
		{

		}
		public async void Refresh(TaskEditorVm parent)
		{
			foreach (var operVm in OperatorsList)
			{
				bool[] result;
				var operModel = operVm.OperatorModel;
				var processModel = parent.SelectedProcess == null ? null : parent.SelectedProcess.Model;
				var taskModel = parent.Model;
				var start = parent.Model.StartDateTime;
				var end = parent.Model.EndDateTime;
				//set process
				if (parent.SelectedProcess == null)
				{
					result = await Task.Run(() =>
						_operatorDataService.GetOperatorStatus(operModel, processModel, start, end));
				}
				//set task
				else
				{
					result = await Task.Run(() =>
						_operatorDataService.GetOperatorStatus(operModel, taskModel, start, end));
				}

				operVm.IsSelected = result[0];
				operVm.IsInTask = result[1];
				operVm.IsInTimeRange = result[2];
			}
		}
		public void Refresh(ProcessEditorVm parent)
		{
			foreach (var operVm in OperatorsList)
			{
				var procOpers = operVm.OperatorModel.ProcessOperators.Where(x =>
					x.Process.Task.StartDateTime < parent.Model.Task.EndDateTime &&
					x.Process.Task.EndDateTime > parent.Model.Task.StartDateTime);

				operVm.IsSelected = procOpers.Any(x =>
					x.Process.Id == parent.Model.Id);
				operVm.IsInTask = procOpers.Any(x => 
					x.Process.Task.Id == parent.Model.Task.Id
					&& x.Process.Id != parent.Model.Id);
			}

			OperatorsSelectedList.Clear();
			foreach (var procOper in parent.Model.ProcessOperators)
			{
				OperatorsSelectedList.Add(new OperatorEditorVm(procOper));
			}
		}
		#region Operators Lists
		public ObservableCollection<OperatorEditorVm> OperatorsList { get { return _operatorsList; } }
		private ObservableCollection<OperatorEditorVm> _operatorsList = new ObservableCollection<OperatorEditorVm>();
		public ObservableCollection<OperatorEditorVm> OperatorsSelectedList { get { return _operatorsSelectedList; } }
		private ObservableCollection<OperatorEditorVm> _operatorsSelectedList = new ObservableCollection<OperatorEditorVm>();
		#endregion


		/// <summary>
		/// Gets or sets a bindable command to clear search query
		/// </summary>
		public Commands.Command ClearSearchCommand
		{
			get { return (Commands.Command)GetValue(ClearSearchCommandProperty); }
			set { SetValue(ClearSearchCommandProperty, value); }
		}
		public static readonly DependencyProperty ClearSearchCommandProperty =
			DependencyProperty.Register("ClearSearchCommand", typeof(Commands.Command), typeof(OperatorManagerVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to refresh operators
		/// </summary>
		public Commands.Command RefreshCommand
		{
			get { return (Commands.Command)GetValue(RefreshCommandProperty); }
			set { SetValue(RefreshCommandProperty, value); }
		}
		public static readonly DependencyProperty RefreshCommandProperty =
			DependencyProperty.Register("RefreshCommand", typeof(Commands.Command), typeof(OperatorManagerVm), new PropertyMetadata(null));
	}
}
