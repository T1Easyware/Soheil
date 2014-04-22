using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Soheil.Core.Base;

/*
 * X : TreeItemVM 
 * 	Container is null as TreeItemVM 
 * 	Containment is null as TreeItemVM 
 * 	ContentsList is X.Ys as TreeItemVM 
 * XY : TreeItemVM 
 * 	Container is XY.X as TreeItemVM 
 * 	Containment is XY.Y as TreeItemVM 
 * 	ContentsList is XY.Zs as TreeItemVM 
 * XYZ : TreeItemVM 
 * 	Container is XYZ.XY as TreeItemVM 
 * 	Containment is XYZ.Z as TreeItemVM 
 * 	ContentsList is XYZ.Ws as TreeItemVM 
 * XYZW : TreeItemVM 	
 *  ...
 */

namespace Soheil.Core.ViewModels.Fpc
{
	/// <summary>
	/// Abstract view model for heirarchical items in fpc state
	/// Such as <see cref="StateConfigVm"/>, <see cref="StateStationVm"/>, <see cref="StateStationActivityVm"/>, <see cref="StateStationActivityMachineVm"/>
	/// <para>e.g. in StateStationActivityVm:</para>
	/// <para> * Container is an instance of StateStationVm which is the parent of this vm</para>
	/// <para> * Containment is an instance of ActivityVm which this vm is enclosing</para>
	/// <para> * ContentsList is a collection of StateStationActivityMachineVm instances which are the children of this vm</para>
	/// <para>*</para>
	/// <para> * The database diagram is as follows StateStation 1 --- * StateStationActivity * --- 1 Activity</para>
	/// </summary>
	public abstract class TreeItemVm : ViewModelBase
	{
		/// <summary>
		/// Gets the Id of model associated with derived class
		/// <para>Must be overriden</para>
		/// </summary>
		public abstract int Id { get; }

		/// <summary>
		/// Initializes this vm (sets parent)
		/// </summary>
		/// <param name="parentWindowVm"></param>
		public TreeItemVm(FpcWindowVm parentWindowVm)
		{
			Parent = parentWindowVm;
		}

		/// <summary>
		/// Gets a bindable value for parent vm
		/// </summary>
		public FpcWindowVm Parent
		{
			get { return (FpcWindowVm)GetValue(ParentProperty); }
			protected set { SetValue(ParentProperty, value); }
		}
		public static readonly DependencyProperty ParentProperty =
			DependencyProperty.Register("Parent", typeof(FpcWindowVm), typeof(TreeItemVm), new UIPropertyMetadata(null));



		#region Structure
		/// <summary>
		/// Gets or sets a bindable text to display as the effective name
		/// <para>Containment's Name</para>
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			protected set { SetValue(NameProperty, value); }
		}
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(TreeItemVm), new UIPropertyMetadata(""));

		/// <summary>
		/// Gets or sets a bindable value for effective view model enclosed by this vm
		/// <para>TreeItemVm=XYZ => Containment=Z</para>
		/// </summary>
		public IToolboxData Containment
		{
			get { return (IToolboxData)GetValue(ContainmentProperty); }
			set { SetValue(ContainmentProperty, value); Name = value == null ? "" : value.Name; }
		}
		public static readonly DependencyProperty ContainmentProperty =
			DependencyProperty.Register("Containment", typeof(IToolboxData), typeof(TreeItemVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value for parent view model that contains this vm
		/// <para>TreeItemVm=XYZ => Container=XY</para>
		/// </summary>
		public TreeItemVm Container
		{
			get { return (TreeItemVm)GetValue(ContainerProperty); }
			set { SetValue(ContainerProperty, value); }
		}
		public static readonly DependencyProperty ContainerProperty =
			DependencyProperty.Register("Container", typeof(TreeItemVm), typeof(TreeItemVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets a bindable collection of view models which this vm contains
		/// <para>TreeItemVm=XYZ => ContentsList=ObservableCollection&lt;XYZW&gt;</para>
		/// </summary>
		public ObservableCollection<TreeItemVm> ContentsList { get { return _contentsList; } }
		private ObservableCollection<TreeItemVm> _contentsList = new ObservableCollection<TreeItemVm>();
		#endregion

		#region Level info
		/// <summary>
		/// Gets a bindable number that indicates which level of tree does this vm belongs to
		/// </summary>
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
		
		/// <summary>
		/// Gets a bindable text to display as the title of this tree level
		/// </summary>
		public string TitleText
		{
			get { return (string)GetValue(TitleTextProperty); }
			set { SetValue(TitleTextProperty, value); }
		}
		public static readonly DependencyProperty TitleTextProperty =
			DependencyProperty.Register("TitleText", typeof(string), typeof(TreeItemVm), new UIPropertyMetadata(""));

		/// <summary>
		/// Returns a text as title of the given tree level
		/// </summary>
		/// <param name="level">tree level (zero-biased index)</param>
		/// <returns></returns>
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
		
		/// <summary>
		/// Gets a bindable brush to display as the background color of this tree level
		/// </summary>
		public SolidColorBrush BackColor
		{
			get { return (SolidColorBrush)GetValue(BackColorProperty); }
			set { SetValue(BackColorProperty, value); }
		}
		public static readonly DependencyProperty BackColorProperty =
			DependencyProperty.Register("BackColor", typeof(SolidColorBrush), typeof(TreeItemVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Returns a brush as background color of the given tree level
		/// </summary>
		/// <param name="level">tree level (zero-biased index)</param>
		/// <returns></returns>
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

		#region Other (IsFixed, IsExpanded, IsDropIndicator)
		/// <summary>
		/// Gets a bindable value that indicates whether this tree item is in use in PPTable (thus can't be deleted or change vital data)
		/// </summary>
		public bool IsFixed
		{
			get { return (bool)GetValue(IsFixedProperty); }
			protected set { SetValue(IsFixedProperty, value); }
		}
		public static readonly DependencyProperty IsFixedProperty =
			DependencyProperty.Register("IsFixed", typeof(bool), typeof(TreeItemVm), new PropertyMetadata(false));

		/// <summary>
		/// Gets a bindable value that indicates whether this vm is expanded as an expander
		/// <para>Changing the value of this property calls isExpandedChanged(bool) in derived class</para>
		/// </summary>
		public bool IsExpanded
		{
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.Register("IsExpanded", typeof(bool), typeof(TreeItemVm),
			new UIPropertyMetadata(false, (d, e) => ((TreeItemVm)d).isExpandedChanged((bool)e.NewValue)));

		/// <summary>
		/// Gets a bindable value that indicates whether this vm is an instance of <see cref="DropIndicatorVm"/>
		/// </summary>
		public bool IsDropIndicator
		{
			get { return (bool)GetValue(IsDropIndicatorProperty); }
			protected set { SetValue(IsDropIndicatorProperty, value); }
		}
		public static readonly DependencyProperty IsDropIndicatorProperty =
			DependencyProperty.Register("IsDropIndicator", typeof(bool), typeof(TreeItemVm), new UIPropertyMetadata(false));
		#endregion

		#region Virtual Methods
		/// <summary>
		/// Override this method to know when IsExpanded is changed
		/// </summary>
		/// <param name="newValue">true if this vm is expanded</param>
		protected virtual void isExpandedChanged(bool newValue) { }

		/// <summary>
		/// Override this method to specify what to do when this state (or any of its sub items) is deleted
		/// </summary>
		public virtual void Delete() { }
		#endregion
	}
}