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
	public class SkillCenterVm : DependencyObject, ISingularList
	{
		#region Members
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		public AccessType Access { get; set; }

		public ProductDataService ProductDataService { get; set; }
		public ProductGroupDataService ProductGroupDataService { get; set; }

		//Tree Observable Collection
		public ObservableCollection<BaseTreeItemVm> Tree { get { return _tree; } }
		private ObservableCollection<BaseTreeItemVm> _tree = new ObservableCollection<BaseTreeItemVm>();

		//SkillCenterContent Dependency Property
		public SkillCenterContentVm Content
		{
			get { return (SkillCenterContentVm)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof(SkillCenterContentVm), typeof(SkillCenterVm), new UIPropertyMetadata(null));

		//IsLoading Dependency Property
		public bool IsLoading
		{
			get { return (bool)GetValue(IsLoadingProperty); }
			set { SetValue(IsLoadingProperty, value); }
		}
		public static readonly DependencyProperty IsLoadingProperty =
			DependencyProperty.Register("IsLoading", typeof(bool), typeof(SkillCenterVm), new UIPropertyMetadata(true));
		//Message Dependency Property
		public EmbeddedException Message
		{
			get { return (EmbeddedException)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}
		public static readonly DependencyProperty MessageProperty =
			DependencyProperty.Register("Message", typeof(EmbeddedException), typeof(SkillCenterVm), new UIPropertyMetadata(null));

		#endregion

		#region Ctor Init Load
		public SkillCenterVm(AccessType access)
		{
			Access = access;
			Message = new EmbeddedException();
			InitializeData();
			initializeCommands();
		}
		private void InitializeData()
		{
			var uow = new Dal.SoheilEdmContext();
			ProductGroupDataService = new ProductGroupDataService(uow);
			ProductDataService = new ProductDataService(uow);

			//Create Data

			//add general first
			var general = new GeneralVm();
			general.Selected += SelectNode;
			Tree.Add(general);

			//add all groups and their children
			var allGroups = ProductGroupDataService.GetActivesRecursive();
			foreach (var productGroup in allGroups)
			{
				var pg = new ProductGroupVm(productGroup);
				pg.Selected += SelectNode;
				Tree.Add(pg);
			}

			IsLoading = false;
		}
		public void SelectNode(BaseTreeItemVm node)
		{
			if (IsLoading) return;
			IsLoading = true;
			_loadingTimer = new Timer(
				o => { 
					Dispatcher.Invoke(() => 
					{
						Content = new SkillCenterContentVm(node);
						Content.ErrorOccured += msg => Message.AddEmbeddedException(msg);
						IsLoading = false;
					});
				}
				, null, 100, System.Threading.Timeout.Infinite);
		}
		Timer _loadingTimer;
		#endregion

		#region Commands
		void initializeCommands()
		{
			RefreshAllCommand = new Commands.Command(o => { });
			CloseMessageCommand = new Commands.Command(o => Message.ResetEmbeddedException());
		}
		//RefreshAllCommand Dependency Property
		public Commands.Command RefreshAllCommand
		{
			get { return (Commands.Command)GetValue(RefreshAllCommandProperty); }
			set { SetValue(RefreshAllCommandProperty, value); }
		}
		public static readonly DependencyProperty RefreshAllCommandProperty =
			DependencyProperty.Register("RefreshAllCommand", typeof(Commands.Command), typeof(SkillCenterVm), new UIPropertyMetadata(null)); 

				//CloseMessageCommand Dependency Property
		public Commands.Command CloseMessageCommand
		{
			get { return (Commands.Command)GetValue(CloseMessageCommandProperty); }
			set { SetValue(CloseMessageCommandProperty, value); }
		}
		public static readonly DependencyProperty CloseMessageCommandProperty =
			DependencyProperty.Register("CloseMessageCommand", typeof(Commands.Command), typeof(SkillCenterVm), new UIPropertyMetadata(null));

		#endregion
	}
}
