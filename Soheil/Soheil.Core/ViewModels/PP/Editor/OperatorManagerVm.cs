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
		/// <summary>
		/// Occurs when any operator is selected/deselected
		/// <para>first param: viewModel, second param: newValue for IsSelected, third param: value that indicates whether to update SelectedOperatorCount</para>
		/// </summary>
		public event Action<OperatorEditorVm, bool, bool> SelectionChanged;

		#region Data
		Dal.SoheilEdmContext _uow;
		public DataServices.OperatorDataService OperatorDataService { get; private set; }
		/// <summary>
		/// Gets or sets the working block of this manager (to find relative operators status)
		/// </summary>
		public Model.Block Block { get; set; }
		/// <summary>
		/// Gets the working Process of this manager (to find relative operators status)
		/// </summary>
		public Model.Process Process { get; protected set; }
		/// <summary>
		/// Gets the working Activity of this manager (to find relative operators status)
		/// </summary>
		public Model.Activity Activity { get; protected set; } 
		#endregion

		#region Ctor and init
		public OperatorManagerVm(Dal.SoheilEdmContext uow)
		{
			_uow = uow;

			#region get all operators and convert them to VM
			OperatorDataService = new DataServices.OperatorDataService(_uow);
			var allOperators = OperatorDataService.GetActives();
			foreach (var oper in allOperators)
			{
				var operVm = new OperatorEditorVm(oper);

				//Notify
				// Add|Remove selected|deselected operator in vm
				operVm.SelectedOperatorChanged += Operator_SelectedOperatorChanged;

				//Updates role in uow
				operVm.OperatorRoleChanged += Operator_RoleChanged;

				OperatorsList.Add(operVm);
			} 
			#endregion

			#region init commands
			ClearSearchCommand = new Commands.Command(textBox =>
			{
				if (textBox is System.Windows.Controls.TextBox)
					(textBox as System.Windows.Controls.TextBox).Clear();
			});

			RefreshCommand = new Commands.Command(o => refresh()); 
			#endregion
		}

		/// <summary>
		/// Updates Process and Activity and Refreshes operators status
		/// </summary>
		/// <param name="processVm"></param>
		public void Refresh(ProcessEditorVm processVm)
		{
			Process = processVm.Model;
			Activity = processVm.ActivityModel;
			refresh();
		}
		/// <summary>
		/// Refreshes current process' operators status
		/// </summary>
		internal async void refresh()
		{
			if (Process != null) if (Process.Task == null) return;

			foreach (var oper in OperatorsList)
			{
				try
				{
					bool[] status;
					var operModel = oper.OperatorModel;
					if (Process == null)
					{
						status = await Task.Run(() => OperatorDataService.GetOperatorStatus(operModel, Block.Tasks.FirstOrDefault()));
					}
					else
					{
						status = await Task.Run(() => OperatorDataService.GetOperatorStatus(operModel, Process));
					}
					oper.IsSelected = status[0];
					oper.IsInTask = status[1];
					oper.IsInTimeRange = status[2];
				}
				catch { }
			}
		} 
		#endregion

		#region Event Handlers
		void Operator_SelectedOperatorChanged(OperatorEditorVm operVm, bool isSelected, bool updateCount)
		{
			if(Process == null || Activity == null || Block == null)
			{
				MessageBox.Show("ابتدا فعالیت را انتخاب کنید");
				return;
			}

			//find ProcessOperator in uow
			var poModel = Process.ProcessOperators.FirstOrDefault(x => x.Operator.Id == operVm.OperatorId);
			//add/remove them
			if (poModel == null)
			{
				if (isSelected)
				{
					//if not exist but selected, add it to uow
					poModel = new Model.ProcessOperator
					{
						Operator = operVm.OperatorModel,
						Process = Process,
						Code = Process.Code + operVm.Code,
					};
					Process.ProcessOperators.Add(poModel);
				}
			}
			else
			{
				if (!isSelected)
				{
					//if exist but not selected, remove it from uow
					Process.ProcessOperators.Remove(poModel);
					new Dal.Repository<Model.ProcessOperator>(_uow).Delete(poModel);
				}
			}

			//notify about selection (to update operators quicklist and SelectedOperatorsCount in process)
			if (SelectionChanged != null)
				SelectionChanged(operVm, isSelected, updateCount);

			//update OperatorsSelectedList
			if (isSelected)
			{
				OperatorsSelectedList.Add(operVm);
			}
			else
			{
				OperatorsSelectedList.Remove(operVm);
			}
		}
		void Operator_RoleChanged(OperatorVm operVm, Soheil.Common.OperatorRole role)
		{
			//find ProcessOperator in uow
			var poModel = Process.ProcessOperators.FirstOrDefault(x => x.Operator.Id == operVm.OperatorId);
			if (poModel != null)
			{
				poModel.Role = role;
			}
			else
			{
				MessageBox.Show("Error: operator is not found");
			}
		} 
		#endregion

		public ObservableCollection<OperatorEditorVm> OperatorsList { get { return _operatorsList; } }
		private ObservableCollection<OperatorEditorVm> _operatorsList = new ObservableCollection<OperatorEditorVm>();
		public ObservableCollection<OperatorEditorVm> OperatorsSelectedList { get { return _operatorsSelectedList; } }
		private ObservableCollection<OperatorEditorVm> _operatorsSelectedList = new ObservableCollection<OperatorEditorVm>();


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
