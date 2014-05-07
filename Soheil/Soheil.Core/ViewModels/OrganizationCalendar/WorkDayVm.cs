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
	/// <summary>
	/// ViewModel for <see cref="Soheil.Model.WorkDay"/> that contains a collection of WorkShiftVms
	/// </summary>
	public class WorkDayVm : DependencyObject
	{
		/// <summary>
		/// Creates an instance of <see cref="WorkDayVm "/> with all its WorkShifts
		/// </summary>
		/// <param name="model"></param>
		/// <param name="prototypes">chooses its instance of prototype from this collection</param>
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

		private Model.WorkDay _model;

		/// <summary>
		/// Gets the Id of this WorkDay
		/// </summary>
		public int Id { get { return _model.Id; } }
		
		/// <summary>
		/// Gets or sets the bindable name of this WorkDay
		/// </summary>
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
		
		/// <summary>
		/// Gets or sets the bindable color for this WorkDay
		/// </summary>
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
		
		/// <summary>
		/// Gets or sets a bindable value that indicates the openness state of business for this WorkDay
		/// <para>Changing the value updates model's BusinessState</para>
		/// </summary>
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

		/// <summary>
		/// Gets a bindable collection for WorkShifts in this WorkDay (equal to number of shift allowed in prototype)
		/// </summary>
		public ObservableCollection<WorkShiftVm> Shifts { get { return _shifts; } }
		private ObservableCollection<WorkShiftVm> _shifts = new ObservableCollection<WorkShiftVm>();
	}
}
