using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.OrganizationCalendar
{
	public class WorkBreakVm : DependencyObject
	{
		public WorkBreakVm(Model.WorkBreak model)
		{
			Model = model;
			StartSeconds = model.StartSeconds;
			EndSeconds = model.EndSeconds;
			_freezeSeconds = false;
		}

		public Model.WorkBreak Model { get; private set; }
		public int Id { get { return Model.Id; } }

		private bool _freezeSeconds = true;

		//StartSeconds Dependency Property
		public int StartSeconds
		{
			get { return (int)GetValue(StartSecondsProperty); }
			set { SetValue(StartSecondsProperty, value); }
		}
		public static readonly DependencyProperty StartSecondsProperty =
			DependencyProperty.Register("StartSeconds", typeof(int), typeof(WorkBreakVm),
			new UIPropertyMetadata(SoheilConstants.EDITOR_START_SECONDS, (d, e) =>
			{
				var vm = (WorkBreakVm)d;
				var val = (int)e.NewValue;
				vm.Model.StartSeconds = val;
			}, (d, v) =>
			{
				var vm = (WorkBreakVm)d;
				var val = (int)v;
				val = SoheilFunctions.RoundFiveMinutes(val);
				if (vm._freezeSeconds) return val;
				if (val > vm.EndSeconds) return vm.EndSeconds;
				return val;
			}));
		//EndSeconds Dependency Property
		public int EndSeconds
		{
			get { return (int)GetValue(EndSecondsProperty); }
			set { SetValue(EndSecondsProperty, value); }
		}
		public static readonly DependencyProperty EndSecondsProperty =
			DependencyProperty.Register("EndSeconds", typeof(int), typeof(WorkBreakVm),
			new UIPropertyMetadata(SoheilConstants.EDITOR_START_SECONDS, (d, e) =>
			{
				var vm = (WorkBreakVm)d;
				var val = (int)e.NewValue;
				vm.Model.EndSeconds = val;
			}, (d, v) =>
			{
				var vm = (WorkBreakVm)d;
				var val = (int)v;
				val = SoheilFunctions.RoundFiveMinutes(val);
				if (vm._freezeSeconds) return val;
				if (val < vm.StartSeconds) return vm.StartSeconds;
				return val;
			}));

		//DeleteCommand Dependency Property
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(WorkBreakVm), new UIPropertyMetadata(null));
	}
}
