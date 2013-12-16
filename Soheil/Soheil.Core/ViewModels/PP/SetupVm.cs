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

		#region Ctor
		public SetupVm(Model.Setup model, TaskCollection parent) : base(parent)
		{
			_threadLock = new Object();

			Id = model.Id;
			StartDateTime = model.StartDateTime;
			DurationSeconds = model.DurationSeconds;
			RowIndex = model.Warmup.Station.Index;

			EditItemCommand = new Commands.Command(o =>
			{
				Parent.Parent.SelectedNPT = this;
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
					AddEmbeddedException(exp.Message);
				}
			});
			EditReportCommand = new Commands.Command(o =>
			{
				try
				{
					Parent.Parent.SelectedNPT = this;
					IsEditMode = false;
				}
				catch (Exception exp)
				{
					AddEmbeddedException(exp.Message);
				}
			});
		}
		//Thread Functions
		protected override void acqusitionThreadStart()
		{
			try
			{
				Dispatcher.Invoke(new Action(() =>
				{
					if (!NPTDataService.UpdateViewModel(this))
					{
						Parent.RemoveNPT(this);
					}
					else
					{
						Dispatcher.Invoke(acqusitionThreadEnd);
					}
				}));
			}
			catch { }
		}
		protected override void acqusitionThreadEnd()
		{
			ViewMode = Parent.ViewMode;
		} 
		#endregion

		#region Members
		public int ChangeoverId { get; set; }
		public int WarmupId { get; set; }
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

	}
}
