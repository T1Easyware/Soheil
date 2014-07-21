using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PM
{
	public abstract class PmItemBase : DependencyObject
	{
		public event Action<PmItemBase> Selected;
		internal void InvokeSelected() { if (Selected != null)	Selected(this); }

		protected bool _isInitialized = false;

		public abstract int Id { get; set; }

		/// <summary>
		/// Gets or sets a bindable value that indicates Name
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(PmItemBase),
			new PropertyMetadata("", (d, e) => { if (((PmItemBase)d)._isInitialized) ((PmItemBase)d).NameChanged((string)e.NewValue); }));
		/// <summary>
		/// Gets or sets a bindable value that indicates Code
		/// </summary>
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(PmItemBase),
			new PropertyMetadata("", (d, e) => { if (((PmItemBase)d)._isInitialized) ((PmItemBase)d).CodeChanged((string)e.NewValue); }));
		/// <summary>
		/// Gets or sets a bindable value that indicates Description
		/// </summary>
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public static readonly DependencyProperty DescriptionProperty =
			DependencyProperty.Register("Description", typeof(string), typeof(PmItemBase),
			new PropertyMetadata("", (d, e) => { if (((PmItemBase)d)._isInitialized) ((PmItemBase)d).DescriptionChanged((string)e.NewValue); }));
		/// <summary>
		/// Gets or sets a bindable value that indicates Status
		/// </summary>
		public Status Status
		{
			get { return (Status)GetValue(StatusProperty); }
			set { SetValue(StatusProperty, value); }
		}
		public static readonly DependencyProperty StatusProperty =
			DependencyProperty.Register("Status", typeof(Status), typeof(PmItemBase),
			new PropertyMetadata(Status.Active, (d, e) => { if (((PmItemBase)d)._isInitialized) ((PmItemBase)d).StatusChanged((Status)e.NewValue); }));

		/// <summary>
		/// Gets or sets a bindable value that indicates LinkCounter
		/// </summary>
		public int LinkCounter
		{
			get { return (int)GetValue(LinkCounterProperty); }
			set { SetValue(LinkCounterProperty, value); }
		}
		/// <summary>
		/// Updates Link counter automatically (should be overriden if needed)
		/// </summary>
		/// <param name="machineId"></param>
		public virtual void UpdateLinkCounter(PmItemBase linkItemVm) { }
		public static readonly DependencyProperty LinkCounterProperty =
			DependencyProperty.Register("LinkCounter", typeof(int), typeof(PmItemBase), new PropertyMetadata(-1));

		#region CallBacks
		protected abstract void NameChanged(string val);
		protected abstract void CodeChanged(string val);
		protected abstract void DescriptionChanged(string val);
		protected abstract void StatusChanged(Status val);

		#endregion

		/// <summary>
		/// Gets or sets a bindable command that adds this item to collection of a previous page
		/// </summary>
		public Commands.Command UseCommand
		{
			get { return (Commands.Command)GetValue(UseCommandProperty); }
			set { SetValue(UseCommandProperty, value); }
		}
		public static readonly DependencyProperty UseCommandProperty =
			DependencyProperty.Register("UseCommand", typeof(Commands.Command), typeof(PmItemBase), new PropertyMetadata(null));

	}
}
