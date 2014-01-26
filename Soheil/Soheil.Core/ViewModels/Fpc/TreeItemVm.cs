using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Soheil.Core.Base;

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
	public abstract class TreeItemVm : ViewModelBase
	{
		/// <summary>
		/// Must be overriden if Id is extracted directly from the Model
		/// </summary>
		public abstract int Id { get; }

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
		public IToolboxData Containment
		{
			get { return (IToolboxData)GetValue(ContainmentProperty); }
			set { SetValue(ContainmentProperty, value); Name = value == null ? "" : value.Name; }
		}
		public static readonly DependencyProperty ContainmentProperty =
			DependencyProperty.Register("Containment", typeof(IToolboxData), typeof(TreeItemVm), new UIPropertyMetadata(null));

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
					return new SolidColorBrush(Color.FromRgb(150, 150, 150)) { Opacity = 0.5 };
				case 1:
					return new SolidColorBrush(Color.FromRgb(220, 220, 180)) { Opacity = 0.5 };
				case 2:
					return new SolidColorBrush(Color.FromRgb(150, 210, 220)) { Opacity = 0.5 };
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
			DependencyProperty.Register("IsExpanded", typeof(bool), typeof(TreeItemVm),
			new UIPropertyMetadata(false, (d, e) => ((TreeItemVm)d).isExpandedChanged((bool)e.NewValue)));

		//IsDropIndicator Dependency Property
		public bool IsDropIndicator
		{
			get { return (bool)GetValue(IsDropIndicatorProperty); }
			set { SetValue(IsDropIndicatorProperty, value); }
		}
		public static readonly DependencyProperty IsDropIndicatorProperty =
			DependencyProperty.Register("IsDropIndicator", typeof(bool), typeof(TreeItemVm), new UIPropertyMetadata(false));
		#endregion
		
		#region Virtual Methods
		protected virtual void isExpandedChanged(bool newValue) { }
		
		public virtual void Change() { }

		public virtual void Delete() { }
		#endregion
	}
}
