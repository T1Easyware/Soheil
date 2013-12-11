using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Interfaces;

namespace Soheil.Core.ViewModels.SetupTime
{
	public class SetupTimeTableVm : DependencyObject, ISingularList
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public AccessType Access { get; private set; }
		public SetupTimeTableVm(AccessType access)
		{
			Access = access;

			RefreshAllCommand = new Commands.Command(o =>
			{
				var models = new DataServices.StationDataService().GetActives();
				foreach (var model in models)
				{
					Stations.Add(new Station(model));
				}
				if (Stations.Any())
				{
					if (SelectedStation != null)
					{
						SelectedStation = Stations.FirstOrDefault(x => x.Id == SelectedStation.Id);
						if (SelectedStation == null)
							SelectedStation = Stations.First();
					}
					else
						SelectedStation = Stations.First();
				}
			});

			RefreshAllCommand.Execute(null);
		}

		//Stations Observable Collection
		private ObservableCollection<Station> _stations = new ObservableCollection<Station>();
		public ObservableCollection<Station> Stations { get { return _stations; } }
		//SelectedStation Dependency Property
		public Station SelectedStation
		{
			get { return (Station)GetValue(SelectedStationProperty); }
			set { SetValue(SelectedStationProperty, value); }
		}
		public static readonly DependencyProperty SelectedStationProperty =
			DependencyProperty.Register("SelectedStation", typeof(Station), typeof(SetupTimeTableVm),
			new PropertyMetadata(null, (d, e) => ((Station)e.NewValue).Reload()));
		//RefreshAllCommand Dependency Property
		public Commands.Command RefreshAllCommand
		{
			get { return (Commands.Command)GetValue(RefreshAllCommandProperty); }
			set { SetValue(RefreshAllCommandProperty, value); }
		}
		public static readonly DependencyProperty RefreshAllCommandProperty =
			DependencyProperty.Register("RefreshAllCommand", typeof(Commands.Command), typeof(SetupTimeTableVm), new UIPropertyMetadata(null));
	}
}
