using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.OrganizationCalendar
{
	/// <summary>
	/// ViewModel for <see cref="Soheil.Model.WorkShiftPrototype"/> which has its color selected from <see cref="WorkProfileVm"/>'s ShiftColors collection
	/// </summary>
	public class WorkShiftPrototypeVm : DependencyObject
	{
		/// <summary>
		/// Creates an instance of WorkShiftPrototypeVm with given model and chooses the instance of shiftcolor for it
		/// </summary>
		/// <param name="model"></param>
		/// <param name="shiftColors">Collection of shift colors could be modified after ctor</param>
		public WorkShiftPrototypeVm(Model.WorkShiftPrototype model, ICollection<ShiftColorVm> shiftColors)
		{
			_model = model;
			Name = _model.Name;
			Index = _model.Index;

			SelectedColor = shiftColors.FirstOrDefault(x => x.Color == model.Color);
			if (SelectedColor == null)
			{
				var custom = new ShiftColorVm { Color = model.Color, Name = "Custom" };
				shiftColors.Add(custom);
			}
		}

		private Model.WorkShiftPrototype _model;
		/// <summary>
		/// Gets Id of this prototype
		/// </summary>
		public int Id { get { return _model.Id; } }

		/// <summary>
		/// Gets or sets the bindable text for name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(WorkShiftPrototypeVm), new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (WorkShiftPrototypeVm)d;
				var val = (string)e.NewValue;
				vm._model.Name = val;
			}));

		/// <summary>
		/// Gets or sets the bindable instance of ShiftColor for SelectedColor of this prototype
		/// </summary>
		public ShiftColorVm SelectedColor
		{
			get { return (ShiftColorVm)GetValue(SelectedColorProperty); }
			set { SetValue(SelectedColorProperty, value); }
		}
		public static readonly DependencyProperty SelectedColorProperty =
			DependencyProperty.Register("SelectedColor", typeof(ShiftColorVm), typeof(WorkShiftPrototypeVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (WorkShiftPrototypeVm)d;
				var val = (ShiftColorVm)e.NewValue;
				if (val == null) return;
				vm._model.Color = val.Color;
			}));

		/// <summary>
		/// Zero-biased index of this shift prototype in its WorkProfile where it's being used
		/// </summary>
		public int Index
		{
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public static readonly DependencyProperty IndexProperty =
			DependencyProperty.Register("Index", typeof(int), typeof(WorkShiftPrototypeVm), new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (WorkShiftPrototypeVm)d;
				var val = (byte)(int)e.NewValue;
				vm._model.Index = val;
			}));

	}
}
