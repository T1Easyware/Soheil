using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels.SetupTime;

namespace Soheil.Core.ViewModels
{
    public class UnitRelationTable : DependencyObject, ISingularList
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public UnitRelationTable()
		{
			RefreshAllCommand = new Commands.Command(o =>
			{
				var models = new DataServices.UnitConversionDataService().GetActives();
				foreach (var model in models)
				{
					UnitConversions.Add(new UnitRelation(model));
				}
				if (UnitConversions.Any())
				{
					if (SelectedUnitConversion != null)
					{
						SelectedUnitConversion = UnitConversions.FirstOrDefault(x => x.Id == SelectedUnitConversion.Id);
					}
				}
			});

			RefreshAllCommand.Execute(null);
		}

		/// <summary>
		/// Gets a bindable collection of UnitConversions
		/// </summary>
		public ObservableCollection<UnitRelation> UnitConversions { get { return _stations; } }
		private ObservableCollection<UnitRelation> _stations = new ObservableCollection<UnitRelation>();
		/// <summary>
		/// Gets or sets the bindable selected station
		/// </summary>
		public UnitRelation SelectedUnitConversion
		{
			get { return (UnitRelation)GetValue(SelectedUnitConversionProperty); }
			set { SetValue(SelectedUnitConversionProperty, value); }
		}
		public static readonly DependencyProperty SelectedUnitConversionProperty =
			DependencyProperty.Register("SelectedUnitConversion", typeof(UnitRelation), typeof(SetupTimeTableVm),
			new PropertyMetadata(null, (d, e) => ((UnitRelation)e.NewValue).Reload()));
		/// <summary>
		/// Gets or sets a bindable command to refresh everything
		/// </summary>
		public Commands.Command RefreshAllCommand
		{
			get { return (Commands.Command)GetValue(RefreshAllCommandProperty); }
			set { SetValue(RefreshAllCommandProperty, value); }
		}
		public static readonly DependencyProperty RefreshAllCommandProperty =
			DependencyProperty.Register("RefreshAllCommand", typeof(Commands.Command), typeof(SetupTimeTableVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates CanEdit
		/// </summary>
		public bool CanEdit
		{
			get { return (bool)GetValue(CanEditProperty); }
			set { SetValue(CanEditProperty, value); }
		}
		public static readonly DependencyProperty CanEditProperty =
			DependencyProperty.Register("CanEdit", typeof(bool), typeof(SetupTimeTableVm), new PropertyMetadata(true));

	}
}