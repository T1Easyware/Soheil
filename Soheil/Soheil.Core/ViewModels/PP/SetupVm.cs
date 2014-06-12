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
		public int ChangeoverId { get; set; }
		public int WarmupId { get; set; }
		public Model.Setup Model { get; protected set; }
		public override int Id { get { return Model.Id; } }
		private int _id;


		#region Ctor
		public SetupVm(int setupId, PPItemCollection parent) 
			: base(parent)
		{
			UOW = new Dal.SoheilEdmContext();
			_id = setupId;
			reloadFromModel();
			initializeCommands();
		}
		public override void Reload(PPItemNpt item)
		{
			reloadFromModel();//???
		}
		void reloadFromModel()
		{
			//update model from database with new UOW
			Model = new Dal.Repository<Model.NonProductiveTask>(UOW).OfType<Model.Setup>().Single(x => x.Id == _id);

			StartDateTime = Model.StartDateTime;
			EndDateTime = Model.EndDateTime;
			DurationSeconds = Model.DurationSeconds;
			RowIndex = Model.Warmup.Station.Index;

			ChangeoverId = Model.Changeover.Id;
			ChangeoverSeconds = Model.Changeover.Seconds;

			WarmupId = Model.Warmup.Id;
			WarmupSeconds = Model.Warmup.Seconds;

			FromProduct = new ViewModels.PP.ProductReworkVm(Model.Changeover.FromProductRework);
			ToProduct = new ViewModels.PP.ProductReworkVm(Model.Changeover.ToProductRework);
		}
		#endregion

		#region prop dps
		//EndDateTime Dependency Property
		public DateTime EndDateTime
		{
			get { return (DateTime)GetValue(EndDateTimeProperty); }
			set { SetValue(EndDateTimeProperty, value); }
		}
		public static readonly DependencyProperty EndDateTimeProperty =
			DependencyProperty.Register("EndDateTime", typeof(DateTime), typeof(SetupVm), new UIPropertyMetadata(DateTime.Now));

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
			var prev = new DataServices.BlockDataService(UOW).FindPreviousBlock(Model.Warmup.Station.Id, StartDateTime);
			if (prev.Item1 != null) return prev.Item1.EndDateTime;
			return null;
		}
		DateTime? lastestTime()
		{
			var next = new DataServices.BlockDataService(UOW).FindNextBlock(Model.Warmup.Station.Id, StartDateTime.AddSeconds(1));
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
					Model.StartDateTime = StartDateTime;
					reloadFromModel();
				}
			});
			CancelCommand = new Commands.Command(o => { StartDateTime = Model.StartDateTime; IsEditMode = false; });
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
					new DataServices.NPTDataService().DeleteModel(Id);
					Parent.RemoveItem(this);
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
