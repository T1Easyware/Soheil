using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Soheil.Common;

namespace Soheil.Core.ViewModels.OrganizationCalendar
{
	public class WorkDayVm : DependencyObject
	{
		public WorkDayVm(Model.WorkDay model, ICollection<WorkShiftPrototypeVm> prototypes)
		{
			_model = model;
			Name = model.Name;
			Color = model.Color;
			BusinessState = model.BusinessState;
			foreach (var workShift in model.WorkShifts)
			{
				Shifts.Add(new WorkShiftVm(workShift, prototypes.First(x => x.Id == workShift.WorkShiftPrototype.Id)));
			}
		}
		protected WorkDayVm() { }

		private Model.WorkDay _model;
		public int Id { get { return _model.Id; } }
		//Name Dependency Property
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(WorkDayVm),
			new UIPropertyMetadata("", (d, e) =>
			{
				var vm = (WorkDayVm)d;
				var val = (string)e.NewValue;
				vm._model.Name = val;
			}));
		//Color Dependency Property
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(WorkDayVm)
			, new UIPropertyMetadata(Colors.Transparent, (d, e) =>
			{
				var vm = (WorkDayVm)d;
				var val = (Color)e.NewValue;
				vm._model.Color = val;
			}));
		//State Dependency Property
		public BusinessDayType BusinessState
		{
			get { return (BusinessDayType)GetValue(BusinessStateProperty); }
			set { SetValue(BusinessStateProperty, value); }
		}
		public static readonly DependencyProperty BusinessStateProperty =
			DependencyProperty.Register("BusinessState", typeof(BusinessDayType), typeof(WorkDayVm),
			new UIPropertyMetadata(BusinessDayType.Open, (d, e) =>
			{
				var vm = (WorkDayVm)d;
				var val = (BusinessDayType)e.NewValue;
				vm._model.BusinessState = val;
			}));

		//Shifts Observable Collection
		private ObservableCollection<WorkShiftVm> _shifts = new ObservableCollection<WorkShiftVm>();
		public ObservableCollection<WorkShiftVm> Shifts { get { return _shifts; } }
	}
}
