using Soheil.Common;
using Soheil.Common.SoheilException;
using Soheil.Core.Base;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace Soheil.Core.ViewModels.SkillCenter
{
	/// <summary>
	/// ViewModel for SkillCenter View
	/// </summary>
	/// <remarks>Uses a local UnitOfWork</remarks>
	public class SkillCenterVm : DependencyObject, ISingularList
	{
		#region Members
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public AccessType Access { get; set; }

		public ProductDataService ProductDataService { get; set; }
		public ProductGroupDataService ProductGroupDataService { get; set; }

		/// <summary>
		/// Gets a (first level of tree) collection of <see cref="BaseTreeItemVm"/>
		/// <para>In order to see the contents, user must select a node from this tree using SelectNode()</para>
		/// </summary>
		public ObservableCollection<BaseTreeItemVm> Tree { get { return _tree; } }
		private ObservableCollection<BaseTreeItemVm> _tree = new ObservableCollection<BaseTreeItemVm>();

		/// <summary>
		/// Gets the bindable contents of the selected <see cref="BaseTreeItemVm"/> of this Vm
		/// </summary>
		public SkillCenterContentVm Content
		{
			get { return (SkillCenterContentVm)GetValue(ContentProperty); }
			protected set { SetValue(ContentProperty, value); }
		}
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof(SkillCenterContentVm), typeof(SkillCenterVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets a bindable value that indicates whether this Vm is Loading...
		/// </summary>
		public bool IsLoading
		{
			get { return (bool)GetValue(IsLoadingProperty); }
			protected set { SetValue(IsLoadingProperty, value); }
		}
		public static readonly DependencyProperty IsLoadingProperty =
			DependencyProperty.Register("IsLoading", typeof(bool), typeof(SkillCenterVm), new UIPropertyMetadata(true));

		/// <summary>
		/// Gets the bindable <see cref="EmbeddedException"/> for error messages about this Vm
		/// </summary>
		public EmbeddedException Message
		{
			get { return (EmbeddedException)GetValue(MessageProperty); }
			protected set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(EmbeddedException), typeof(SkillCenterVm), new UIPropertyMetadata(null));

		#endregion

		#region Ctor Init Load
		/// <summary>
		/// Instantiates an instance of SkillCenterVm and initializes its DataServices and Commands and loads basic data
		/// </summary>
		public SkillCenterVm(AccessType access)
		{
			Access = access;
			Message = new EmbeddedException();
			InitializeData();
			initializeCommands();
		}

		/// <summary>
		/// Creates a UOW and initializes DataServices and adds ProductGroups and stuff
		/// </summary>
		private void InitializeData()
		{
			var uow = new Dal.SoheilEdmContext();
			ProductGroupDataService = new ProductGroupDataService(uow);
			ProductDataService = new ProductDataService(uow);

			//Create Tree

			//add general node first
			var general = new GeneralVm();
			general.Selected += SelectNode;
			Tree.Add(general);

			//add all groups and their children recursively
			var allGroups = ProductGroupDataService.GetActives();
			foreach (var productGroup in allGroups)
			{
				var pg = new ProductGroupVm(productGroup);
				pg.Selected += SelectNode;
				Tree.Add(pg);
			}

			IsLoading = false;
		}

		/// <summary>
		/// Selects the given <see cref="BaseTreeItemVm"/> and changes the Content to show the skills about that <see cref="BaseTreeItemVm"/>
		/// </summary>
		/// <remarks>
		/// This method will be called from the Selected event in each <see cref="BaseTreeItemVm"/>
		/// <para>This method is using a timer to load content with a delay</para>
		/// </remarks>
		/// <param name="node">The node which skills will be shown</param>
		public void SelectNode(BaseTreeItemVm node)
		{
			//exit if page is in Loading state
			if (IsLoading) return;
			
			IsLoading = true;

			//load the content with a delay
			if (_loadingTimer != null) _loadingTimer.Dispose();
			_loadingTimer = new Timer(
				o => { 
					Dispatcher.Invoke(() => 
					{
						//Reset the content to match the selected node
						Content = new SkillCenterContentVm(node);
						//prepare this page's Message to listen to errors of the Content
						Content.ErrorOccured += msg => Message.AddEmbeddedException(msg);
						IsLoading = false;
					});
				}
				, null, _loadingTimerDelay, System.Threading.Timeout.Infinite);
		}
		/// <summary>
		/// Delay for the _loadingTimer (= 100)
		/// </summary>
		const int _loadingTimerDelay = 100;
		/// <summary>
		/// A timer to load the content with a delay
		/// </summary>
		Timer _loadingTimer;
		#endregion

		#region Commands
		/// <summary>
		/// Initializes the commands of this Vm
		/// </summary>
		void initializeCommands()
		{
			RefreshAllCommand = new Commands.Command(o => { throw new NotImplementedException(); });//???
			CloseMessageCommand = new Commands.Command(o => Message.ResetEmbeddedException());
		}

		/// <summary>
		/// Gets a bindable command that Refreshes everything on this Vm
		/// </summary>
		public Commands.Command RefreshAllCommand
		{
			get { return (Commands.Command)GetValue(RefreshAllCommandProperty); }
			protected set { SetValue(RefreshAllCommandProperty, value); }
		}
		public static readonly DependencyProperty RefreshAllCommandProperty =
			DependencyProperty.Register("RefreshAllCommand", typeof(Commands.Command), typeof(SkillCenterVm), new UIPropertyMetadata(null)); 

		/// <summary>
		/// Gets a bindable command that Closes the error message
		/// </summary>
		public Commands.Command CloseMessageCommand
		{
			get { return (Commands.Command)GetValue(CloseMessageCommandProperty); }
			protected set { SetValue(CloseMessageCommandProperty, value); }
		}
		public static readonly DependencyProperty CloseMessageCommandProperty =
			DependencyProperty.Register("CloseMessageCommand", typeof(Commands.Command), typeof(SkillCenterVm), new UIPropertyMetadata(null));

		#endregion
	}
}
