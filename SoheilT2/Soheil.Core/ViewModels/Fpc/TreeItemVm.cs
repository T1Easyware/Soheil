using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Soheil.Core.ViewModels.Fpc
{
	//X : TreeItemVM 
	//	Container is null as TreeItemVM 
	//	Containment is null as TreeItemVM 
	//	ContentsList is X.Ys as TreeItemVM 
	//XY : TreeItemVM 
	//	Container is XY.X as TreeItemVM 
	//	Containment is XY.Y as TreeItemVM 
	//	ContentsList is XY.Zs as TreeItemVM 
	//XYZ : TreeItemVM 
	//	Container is XYZ.XY as TreeItemVM 
	//	Containment is XYZ.Z as TreeItemVM 
	//	ContentsList is XYZ.Ws as TreeItemVM 
	//XYZW : TreeItemVM 
	//...
	public class TreeItemVm : ViewModel
	{
		public TreeItemVm(FpcWindowVm parentWindowVm)
		{
			Parent = parentWindowVm;
		}

		//Parent Dependency Property
		public FpcWindowVm Parent
		{
			get { return (FpcWindowVm)GetValue(ParentProperty); }
			set { SetValue(ParentProperty, value); }
		}
		public static readonly DependencyProperty ParentProperty =
			DependencyProperty.Register("Parent", typeof(FpcWindowVm), typeof(TreeItemVm), new UIPropertyMetadata(null));

		#region Structure
		/// <summary>
		/// Effective name of Containment
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(TreeItemVm), new UIPropertyMetadata(""));

		/// <summary>
		/// Containment in TreeItemVm=XYZ is Z
		/// </summary>
		public NamedVM Containment
		{
			get { return (NamedVM)GetValue(ContainmentProperty); }
			set { SetValue(ContainmentProperty, value); Name = value == null ? "" : value.Name; }
		}
		public static readonly DependencyProperty ContainmentProperty =
			DependencyProperty.Register("Containment", typeof(NamedVM), typeof(TreeItemVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Container in TreeItemVm=XYZ is XY
		/// </summary>
		public TreeItemVm Container
		{
			get { return (TreeItemVm)GetValue(ContainerProperty); }
			set { SetValue(ContainerProperty, value); }
		}
		public static readonly DependencyProperty ContainerProperty =
			DependencyProperty.Register("Container", typeof(TreeItemVm), typeof(TreeItemVm), new UIPropertyMetadata(null));

		/// <summary>
		/// ContentsList in TreeItemVm=XYZ is XYZW[ ]
		/// </summary>
		public ObservableCollection<TreeItemVm> ContentsList { get { return _contentsList; } }
		private ObservableCollection<TreeItemVm> _contentsList = new ObservableCollection<TreeItemVm>();
		#endregion

		#region Level info
		//TreeLevel Dependency Property
		public int TreeLevel
		{
			get { return (int)GetValue(TreeLevelProperty); }
			set
			{
				SetValue(TreeLevelProperty, value);
				BackColor = GetLevelColor(value);
				TitleText = GetLevelTitle(value);
			}
		}
		public static readonly DependencyProperty TreeLevelProperty =
			DependencyProperty.Register("TreeLevel", typeof(int), typeof(TreeItemVm), new UIPropertyMetadata(0));
		//TitleText Dependency Property
		public string TitleText
		{
			get { return (string)GetValue(TitleTextProperty); }
			set { SetValue(TitleTextProperty, value); }
		}
		public static readonly DependencyProperty TitleTextProperty =
			DependencyProperty.Register("TitleText", typeof(string), typeof(TreeItemVm), new UIPropertyMetadata(""));
		public string GetLevelTitle(int level)
		{
			switch (level)
			{
				case 0:
					return "ایستگاه ها:";
				case 1:
					return "فعالیت ها:";
				case 2:
					return "ماشین ها:";
				default:
					return "";
			}
		}
		//BackColor Dependency Property
		public SolidColorBrush BackColor
		{
			get { return (SolidColorBrush)GetValue(BackColorProperty); }
			set { SetValue(BackColorProperty, value); }
		}
		public static readonly DependencyProperty BackColorProperty =
			DependencyProperty.Register("BackColor", typeof(SolidColorBrush), typeof(TreeItemVm), new UIPropertyMetadata(null));
		public SolidColorBrush GetLevelColor(int level)
		{
			switch (level)
			{
				case 0:
					return new SolidColorBrush(Color.FromRgb(190, 175, 210)) { Opacity = 0.5 };
				case 1:
					return new SolidColorBrush(Color.FromRgb(150, 180, 220)) { Opacity = 0.5 };
				case 2:
					return new SolidColorBrush(Color.FromRgb(160, 200, 180)) { Opacity = 0.5 };
				default:
					return new SolidColorBrush(Colors.Red);
			}
		}
		#endregion

		#region Other
		//IsExpanded Dependency Property
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.Register("IsExpanded", typeof(bool), typeof(TreeItemVm), new UIPropertyMetadata(false, isExpandedChanged));
		public static void isExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!(bool)e.NewValue)
			{
				if (d as StateConfigVm != null)
					(d as StateConfigVm).State.ShowDetails = false;
			}
			else if (!(d is StateConfigVm))
			{
				var conf = (d as TreeItemVm);
				var q = (d as TreeItemVm).Container.ContentsList.Where(x => x.IsExpanded && x != d);
				foreach (var item in q) item.IsExpanded = false;
				if (d is StateStationVm)
					conf.Parent.FocusedStateStation = d as StateStationVm;
				else if (d is StateStationActivityVm)
					conf.Parent.FocusedStateStation = (d as StateStationActivityVm).Container as StateStationVm;
				else if (d is StateStationActivityMachineVm)
					conf.Parent.FocusedStateStation
						= ((d as StateStationActivityMachineVm).Container as StateStationActivityVm).Container as StateStationVm;
				conf.Parent.OnStationSelected(conf.Parent.FocusedStateStation);
			}
		}
		//IsDropIndicator Dependency Property
		public bool IsDropIndicator
		{
			get { return (bool)GetValue(IsDropIndicatorProperty); }
			set { SetValue(IsDropIndicatorProperty, value); }
		}
		public static readonly DependencyProperty IsDropIndicatorProperty =
			DependencyProperty.Register("IsDropIndicator", typeof(bool), typeof(TreeItemVm), new UIPropertyMetadata(false));

		public void Delete()
		{
			if (this is StateConfigVm)
			{
				(this as StateConfigVm).State.DeleteCommand.Execute(null);
			}
			else if (this is StateStationVm)
			{
				this.Container.ContentsList.Remove(this);
			}
			else if (this is StateStationActivityVm)
			{
				this.Container.ContentsList.Remove(this);
			}
			else if (this is StateStationActivityMachineVm)
			{
				this.Container.ContentsList.Remove(this);
			}
		}

		public virtual void Change()
		{
		} 
		#endregion
	}
}
