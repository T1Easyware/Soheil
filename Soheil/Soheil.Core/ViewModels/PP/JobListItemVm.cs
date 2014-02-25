using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP
{
	public class JobListItemVm : PPJobVm
	{
		public event Action<int> JobSelected;

		public JobListItemVm(Model.Job model): base(model)
		{
			initializeCommands();

			BlocksCount = model.Blocks.Count;
			if (BlocksCount > 0)
			{
				StartDT = model.Blocks.OrderBy(x => x.StartDateTime).First().StartDateTime;
				EndDT = model.Blocks.OrderBy(x => x.StartDateTime).Last().EndDateTime;
			}
			ProductRework = new ProductReworkVm(model.ProductRework);
		}

		//Product Dependency Property
		public ProductReworkVm ProductRework
		{
			get { return (ProductReworkVm)GetValue(ProductReworkProperty); }
			set { SetValue(ProductReworkProperty, value); }
		}
		public static readonly DependencyProperty ProductReworkProperty =
			DependencyProperty.Register("ProductRework", typeof(ProductReworkVm), typeof(JobListItemVm), new UIPropertyMetadata(null));
		//BlocksCount Dependency Property
		public int BlocksCount
		{
			get { return (int)GetValue(BlocksCountProperty); }
			set { SetValue(BlocksCountProperty, value); }
		}
		public static readonly DependencyProperty BlocksCountProperty =
			DependencyProperty.Register("BlocksCount", typeof(int), typeof(JobListItemVm), new UIPropertyMetadata(0));
		//StartDT Dependency Property
		public DateTime StartDT
		{
			get { return (DateTime)GetValue(StartDTProperty); }
			set { SetValue(StartDTProperty, value); }
		}
		public static readonly DependencyProperty StartDTProperty =
			DependencyProperty.Register("StartDT", typeof(DateTime), typeof(JobListItemVm), new UIPropertyMetadata(DateTime.Now));
		//EndDT Dependency Property
		public DateTime EndDT
		{
			get { return (DateTime)GetValue(EndDTProperty); }
			set { SetValue(EndDTProperty, value); }
		}
		public static readonly DependencyProperty EndDTProperty =
			DependencyProperty.Register("EndDT", typeof(DateTime), typeof(JobListItemVm), new UIPropertyMetadata(DateTime.Now));

		#region Commands
		void initializeCommands()
		{
			SelectCommand = new Commands.Command(o =>
			{
				if (JobSelected != null) JobSelected(Id);
			});
		}
		//SearchCommand Dependency Property
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(JobListVm), new UIPropertyMetadata(null));
		#endregion

		public void UpdateDescription(string val)
		{
			Model.Description = val;
			new DataServices.JobDataService().UpdateDescriptionOnly(Model);
		}
	}
}
