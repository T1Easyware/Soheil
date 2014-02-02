using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.OrganizationCalendar
{
	public class WorkShiftPrototypeVm : DependencyObject
	{
		public WorkShiftPrototypeVm(Model.WorkShiftPrototype model, ICollection<ShiftColorVm> shiftColors)
		{
			_model = model;
			Name = _model.Name;
			Color = _model.Color;
			SelectedColor = shiftColors.First(x => x.Color == Color);
			Index = _model.Index;
		}

		private Model.WorkShiftPrototype _model;
		public int Id { get { return _model.Id; } }
		//Name Dependency Property
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
		//Color Dependency Property
		public Color Color
		{
			get { return (Color)GetValue(ColorProperty); }
			set { SetValue(ColorProperty, value); }
		}
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(WorkShiftPrototypeVm), new UIPropertyMetadata(Colors.Transparent, (d, e) =>
			{
				var vm = (WorkShiftPrototypeVm)d;
				var val = (Color)e.NewValue;
				vm._model.Color = val;
			}));
		//SelectedColor Dependency Property
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
				vm.Color = val.Color;
			}));
		//Index Dependency Property
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
