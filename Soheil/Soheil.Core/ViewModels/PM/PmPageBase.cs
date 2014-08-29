﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.PM
{
	public class PmPageBase : DependencyObject
	{
		public event Action Refresh;
        public event Action<PmItemBase> SelectedItemChanged;

        internal void RaiseRefresh() { if (Refresh != null) Refresh(); }

		public PmPageBase()
		{
		}

		/// <summary>
		/// Gets or sets a bindable collection that indicates Items
		/// </summary>
		public ObservableCollection<PmItemBase> Items { get { return _items; } }
		private ObservableCollection<PmItemBase> _items = new ObservableCollection<PmItemBase>();

		/// <summary>
		/// Gets or sets a bindable value that indicates SelectedItem
		/// </summary>
		public PmItemBase SelectedItem
		{
			get { return (PmItemBase)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(PmItemBase), typeof(PmPageBase),
			new PropertyMetadata(null, (d, e) =>
			{
				var vm = (PmPageBase)d;
				var val = (PmItemBase)e.NewValue;
                if (val != null && vm.SelectedItemChanged != null)
                    vm.SelectedItemChanged(val);
			}));

		#region Optional flags
		/// <summary>
		/// Gets or sets a bindable value that indicates whether to hide Machine column in GridView
		/// </summary>
		public bool HideMachines
		{
			get { return (bool)GetValue(HideMachinesProperty); }
			set { SetValue(HideMachinesProperty, value); }
		}
		public static readonly DependencyProperty HideMachinesProperty =
			DependencyProperty.Register("HideMachines", typeof(bool), typeof(PmPageBase), new PropertyMetadata(true));
		/// <summary>
		/// Gets or sets a bindable value that indicates whether to hide MachinePart column in GridView
		/// </summary>
		public bool HideMachineParts
		{
			get { return (bool)GetValue(HideMachinePartsProperty); }
			set { SetValue(HideMachinePartsProperty, value); }
		}
		public static readonly DependencyProperty HideMachinePartsProperty =
			DependencyProperty.Register("HideMachineParts", typeof(bool), typeof(PmPageBase), new PropertyMetadata(true));
		/// <summary>
		/// Gets or sets a bindable value that indicates whether to hide MachinePartMaintenance column in GridView
		/// </summary>
		public bool HideMachinePartMaintenances
		{
			get { return (bool)GetValue(HideMachinePartMaintenancesProperty); }
			set { SetValue(HideMachinePartMaintenancesProperty, value); }
		}
		public static readonly DependencyProperty HideMachinePartMaintenancesProperty =
			DependencyProperty.Register("HideMachinePartMaintenances", typeof(bool), typeof(PmPageBase), new PropertyMetadata(true));

		#endregion

		/// <summary>
		/// Gets or sets a bindable command that adds a new source item to this page
		/// </summary>
		public Commands.Command AddCommand
		{
			get { return (Commands.Command)GetValue(AddCommandProperty); }
			set { SetValue(AddCommandProperty, value); }
		}
		public static readonly DependencyProperty AddCommandProperty =
			DependencyProperty.Register("AddCommand", typeof(Commands.Command), typeof(PmPageBase), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable value that indicates DeleteCommand
		/// </summary>
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(PmPageBase), new PropertyMetadata(null));
	}
}