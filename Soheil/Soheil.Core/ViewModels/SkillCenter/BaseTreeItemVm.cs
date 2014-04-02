using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// Basic abstract class to represent a selectable tree item in skill center
	/// <para>Derived types are <see cref="GeneralVm"/>, <see cref="ProductGroupVm"/>, <see cref="ProductVm"/> and <see cref="ProductReworkVm"/></para>
	/// </summary>
	public abstract class BaseTreeItemVm : BaseVm
	{
		/// <summary>
		/// Occures when this tree item is selected
		/// </summary>
		public event Action<BaseTreeItemVm> Selected;
		/// <summary>
		/// Initializes the commands (SelectCommand)
		/// </summary>
		protected void InitializeCommands()
		{
			SelectCommand = new Commands.Command(o =>
			{
				if (Selected != null) Selected(this);
			});
		}
		/// <summary>
		/// Adds a tree item to the children of this tree item
		/// <para>Use this method to handle select event in a proper way</para>
		/// </summary>
		/// <param name="child">child tree item to be added. Selected event handler will be automatically set</param>
		public void AddChild(BaseTreeItemVm child)
		{
			//add the event handler to the new item
			child.Selected += c =>
			{
				if (Selected != null) Selected(c);
			};
			Children.Add(child);
		}
		/// <summary>
		/// Gets a collection of <see cref="BaseTreeItemVm"/>s which are children of the current Vm
		/// <para>Don't manually add to this collection. use AddChild instead</para>
		/// </summary>
		public ObservableCollection<BaseTreeItemVm> Children { get { return _children; } }
		private ObservableCollection<BaseTreeItemVm> _children = new ObservableCollection<BaseTreeItemVm>();
		
		/// <summary>
		/// Gets a bindable command that indicates when this tree item is selected
		/// </summary>
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(BaseTreeItemVm), new UIPropertyMetadata(null));
	}
}
