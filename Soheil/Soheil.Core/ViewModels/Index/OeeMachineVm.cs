using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Soheil.Core.Index;

namespace Soheil.Core.ViewModels.Index
{
	public class OeeMachineVm : DependencyObject
	{
		public Model.Machine Model { get; set; }
		public event Action Selected;
		public OeeMachineVm(Model.Machine model)
		{
			Model = model;
			Name = model.Name;
			_data = new List<OeeRecord>();

			SelectCommand = new Commands.Command(o =>
			{
				if (Selected != null) Selected();
			});
		}

		/// <summary>
		/// View calls this method to load its index infromation from database
		/// </summary>
		public void Load()
		{
			Timeline.Clear();
			for (int i = -5; i < 5; i++)
			{
				var vm = new OeeMachineDetailVm();
				vm.Selected += () =>
				{
					if (SelectedDetail != null)
						SelectedDetail.IsSelected = false;
					SelectedDetail = vm;
				};
				Timeline.Add(vm);

				//load data
				var data = new OeeRecord(Model.Id, DateTime.Now.AddDays(i), DateTime.Now.AddDays(i+1));
				_data.Add(data);
				vm.Load(data);
			}

			SelectedDetail = Timeline.LastOrDefault();
		}

		#region Machine in list
		/// <summary>
		/// Get or sets the bindable name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(OeeMachineVm), new PropertyMetadata(""));

		/// <summary>
		/// Gets or sets a bindable command to Select a machine
		/// </summary>
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(OeeMachineVm), new PropertyMetadata(null)); 
		#endregion

		#region Switch view
		/// <summary>
		/// Gets or sets a bindable value that indicates whether reworks are shown in a different column
		/// </summary>
		public bool ShowReworks
		{
			get { return (bool)GetValue(ShowReworksProperty); }
			set { SetValue(ShowReworksProperty, value); }
		}
		public static readonly DependencyProperty ShowReworksProperty =
			DependencyProperty.Register("ShowReworks", typeof(bool), typeof(OeeMachineVm),
			new PropertyMetadata(true, (d, e) =>
			{
				var vm = (OeeMachineVm)d;
				var val = (bool)e.NewValue;
				if (val)
				{

				}
				else
				{

				}
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates ShowUnreported
		/// </summary>
		public bool ShowUnreported
		{
			get { return (bool)GetValue(ShowUnreportedProperty); }
			set { SetValue(ShowUnreportedProperty, value); }
		}
		public static readonly DependencyProperty ShowUnreportedProperty =
			DependencyProperty.Register("ShowUnreported", typeof(bool), typeof(OeeMachineVm),
			new PropertyMetadata(true, (d, e) =>
			{
				var vm = (OeeMachineVm)d;
				var val = (bool)e.NewValue;
				if (val)
				{

				}
				else
				{

				}
			}));
		#endregion

		#region Timeline and data
		List<OeeRecord> _data;

		public ObservableCollection<OeeMachineDetailVm> Timeline { get { return _timeline; } }
		private ObservableCollection<OeeMachineDetailVm> _timeline = new ObservableCollection<OeeMachineDetailVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates SelectedDetail
		/// </summary>
		public OeeMachineDetailVm SelectedDetail
		{
			get { return (OeeMachineDetailVm)GetValue(SelectedDetailProperty); }
			set { SetValue(SelectedDetailProperty, value); }
		}
		public static readonly DependencyProperty SelectedDetailProperty =
			DependencyProperty.Register("SelectedDetail", typeof(OeeMachineDetailVm), typeof(OeeMachineVm), new PropertyMetadata(null));

		#endregion
	}
}
