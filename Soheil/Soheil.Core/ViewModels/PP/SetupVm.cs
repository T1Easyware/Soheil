using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;
using Soheil.Core.PP;

namespace Soheil.Core.ViewModels.PP
{
	public class SetupVm : NPTVm
	{
		public DataServices.NPTDataService NPTDataService { get { return Parent.NPTDataService; } }
		public int ChangeoverId { get; set; }
		public int WarmupId { get; set; }

		internal Model.Setup SetupModel { get { return _model as Model.Setup; } }

		#region Ctor
		public SetupVm(Model.Setup model, PPItemCollection parent) 
			: base(model, parent)
		{
			StartDateTime = model.StartDateTime;
			DurationSeconds = model.DurationSeconds;
			RowIndex = model.Warmup.Station.Index;

			initializeCommands();
		}
		#endregion

		#region prop dps

		//ChangeoverSeconds Dependency Property
		public int ChangeoverSeconds
		{
			get { return (int)GetValue(ChangeoverSecondsProperty); }
			set { SetValue(ChangeoverSecondsProperty, value); }
		}
		public static readonly DependencyProperty ChangeoverSecondsProperty =
			DependencyProperty.Register("ChangeoverSeconds", typeof(int), typeof(SetupVm), new UIPropertyMetadata(0));
		//WarmupSeconds Dependency Property
		public int WarmupSeconds
		{
			get { return (int)GetValue(WarmupSecondsProperty); }
			set { SetValue(WarmupSecondsProperty, value); }
		}
		public static readonly DependencyProperty WarmupSecondsProperty =
			DependencyProperty.Register("WarmupSeconds", typeof(int), typeof(SetupVm), new UIPropertyMetadata(0));
		//FromProduct Dependency Property
		public ProductReworkVm FromProduct
		{
			get { return (ProductReworkVm)GetValue(FromProductProperty); }
			set { SetValue(FromProductProperty, value); }
		}
		public static readonly DependencyProperty FromProductProperty =
			DependencyProperty.Register("FromProduct", typeof(ProductReworkVm), typeof(SetupVm), new UIPropertyMetadata(null));
		//ToProduct Dependency Property
		public ProductReworkVm ToProduct
		{
			get { return (ProductReworkVm)GetValue(ToProductProperty); }
			set { SetValue(ToProductProperty, value); }
		}
		public static readonly DependencyProperty ToProductProperty =
			DependencyProperty.Register("ToProduct", typeof(ProductReworkVm), typeof(SetupVm), new UIPropertyMetadata(null)); 
		#endregion

		#region Commands
		DateTime? earliestTime()
		{
			var prev = Parent.PPTable.BlockDataService.FindPreviousBlock(SetupModel.Warmup.Station.Id, StartDateTime);
			if (prev.Item1 != null) return prev.Item1.EndDateTime;
			return null;
		}
		DateTime? lastestTime()
		{
			var next = Parent.PPTable.BlockDataService.FindNextBlock(SetupModel.Warmup.Station.Id, StartDateTime.AddSeconds(1));
			if (next.Item1 != null) return next.Item1.StartDateTime.AddSeconds(-DurationSeconds);
			return null;
		}
		protected override void initializeCommands()
		{
			base.initializeCommands();
			SaveCommand = new Commands.Command(o =>
			{
				bool error = false;
				var min = earliestTime();
				if (min.HasValue)
					if (StartDateTime < min.Value)
						error = true;
				var max = lastestTime();
				if (max.HasValue)
					if (StartDateTime > max.Value)
						error = true;
				if (error)
				{
					Message.AddEmbeddedException("زمان وارد شده با برنامه های موجود تداخل دارد");
				}
				else
				{
					SetupModel.StartDateTime = StartDateTime;
					NPTDataService.UpdateViewModel(this);
				}
			});
			CancelCommand = new Commands.Command(o => { StartDateTime = SetupModel.StartDateTime; IsEditMode = false; });
			SetToEarliestTimeCommand = new Commands.Command(o =>
			{
				var dt = earliestTime();
				if (dt.HasValue) StartDateTime = dt.Value;
			});
			SetToLatestTimeCommand = new Commands.Command(o =>
			{
				var dt = lastestTime();
				if (dt.HasValue) StartDateTime = dt.Value;
			});
			EditItemCommand = new Commands.Command(o =>
			{
				Parent.PPTable.SelectedNPT = this;
				IsEditMode = true;
			});
			DeleteItemCommand = new Commands.Command(o =>
			{
				try
				{
					NPTDataService.DeleteModel(Id);
					Parent.RemoveNPT(this);
				}
				catch (Exception exp)
				{
					Message.AddEmbeddedException(exp.Message);
				}
			});
			EditReportCommand = new Commands.Command(o =>
			{
				try
				{
					Parent.PPTable.SelectedNPT = this;
					IsEditMode = false;
				}
				catch (Exception exp)
				{
					Message.AddEmbeddedException(exp.Message);
				}
			});
		}
		//CancelCommand Dependency Property
		public Commands.Command CancelCommand
		{
			get { return (Commands.Command)GetValue(CancelCommandProperty); }
			set { SetValue(CancelCommandProperty, value); }
		}
		public static readonly DependencyProperty CancelCommandProperty =
			DependencyProperty.Register("CancelCommand", typeof(Commands.Command), typeof(SetupVm), new UIPropertyMetadata(null));
		//SetToEarliestTimeCommand Dependency Property
		public Commands.Command SetToEarliestTimeCommand
		{
			get { return (Commands.Command)GetValue(SetToEarliestTimeCommandProperty); }
			set { SetValue(SetToEarliestTimeCommandProperty, value); }
		}
		public static readonly DependencyProperty SetToEarliestTimeCommandProperty =
			DependencyProperty.Register("SetToEarliestTimeCommand", typeof(Commands.Command), typeof(SetupVm), new UIPropertyMetadata(null));
		//SetToLatestTimeCommand Dependency Property
		public Commands.Command SetToLatestTimeCommand
		{
			get { return (Commands.Command)GetValue(SetToLatestTimeCommandProperty); }
			set { SetValue(SetToLatestTimeCommandProperty, value); }
		}
		public static readonly DependencyProperty SetToLatestTimeCommandProperty =
			DependencyProperty.Register("SetToLatestTimeCommand", typeof(Commands.Command), typeof(SetupVm), new UIPropertyMetadata(null));
		#endregion
	}
}
