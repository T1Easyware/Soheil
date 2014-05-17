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
		Dal.SoheilEdmContext _uow;
		DataServices.OperatorDataService _operatorDataService;

		public OperatorManagerVm(Dal.SoheilEdmContext uow)
		{
			_uow = uow;
			_operatorDataService = new DataServices.OperatorDataService(_uow);

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
		public void Refresh(TaskEditorVm parent)
		{
			var allOperators = _operatorDataService.GetActives();
			foreach (var oper in allOperators)
			{
				var procOpers = oper.ProcessOperators.Where(x =>
					x.Process.Task.StartDateTime < parent.Model.EndDateTime &&
					x.Process.Task.EndDateTime < parent.Model.StartDateTime);
				if (procOpers.Any(x => x.Process.Task.Id == parent.Model.Id))
				{

				}
			}
		}

		#region Operators Lists
		public ObservableCollection<ProcessEditorVm> OperatorsIdleList { get { return _operatorsIdleList; } }
		private ObservableCollection<ProcessEditorVm> _operatorsIdleList = new ObservableCollection<ProcessEditorVm>();
		public ObservableCollection<ProcessEditorVm> OperatorsMainList { get { return _operatorsMainList; } }
		private ObservableCollection<ProcessEditorVm> _operatorsMainList = new ObservableCollection<ProcessEditorVm>();
		public ObservableCollection<ProcessEditorVm> OperatorsSubsList { get { return _operatorsSubsList; } }
		private ObservableCollection<ProcessEditorVm> _operatorsSubsList = new ObservableCollection<ProcessEditorVm>();
		public ObservableCollection<ProcessEditorVm> OperatorsAuxList { get { return _operatorsAuxList; } }
		private ObservableCollection<ProcessEditorVm> _operatorsAuxList = new ObservableCollection<ProcessEditorVm>();
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
