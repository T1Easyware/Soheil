using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Soheil.Core.ViewModels.SkillCenter
{
	public class BaseTreeItemVm : BaseVm
	{
		public event Action<BaseTreeItemVm> Selected;
		public BaseTreeItemVm()
		{
			SelectCommand = new Commands.Command(o =>
			{
				if (Selected != null) Selected(this);
			});
		}
		/// <summary>
		/// Use this method to handle select event in a proper way
		/// </summary>
		/// <param name="child"></param>
		public void AddChild(BaseTreeItemVm child)
		{
			child.Selected += c =>
			{
				if (Selected != null) Selected(c);
			};
			Children.Add(child);
		}
		/// <summary>
		/// Don't manually add to this collection. use AddChild instead
		/// </summary>
		public ObservableCollection<BaseTreeItemVm> Children { get { return _children; } }
		private ObservableCollection<BaseTreeItemVm> _children = new ObservableCollection<BaseTreeItemVm>();
		//SelectCommand Dependency Property
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(BaseTreeItemVm), new UIPropertyMetadata(null));
	}
}
