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
	/// <summary>
	/// View model for a selectable item in a <see cref="JobListVm"/>
	/// </summary>
	public class JobListItemVm : PPJobVm
	{
		/// <summary>
		/// Occurs when this item is selected
		/// </summary>
		public event Action<int> JobSelected;

		/// <summary>
		/// Creates an instance of this vm with the given model
		/// </summary>
		/// <param name="model">Product rework and Blocks in this job model are used</param>
		public JobListItemVm(Model.Job model): base(model)
		{
			initializeCommands();

			BlocksCount = model.Blocks.Count;
			//if the job has blocks, set the time range to the first and last of its blocks
			if (BlocksCount > 0)
			{
				StartDT = model.Blocks.OrderBy(x => x.StartDateTime).First().StartDateTime;
				EndDT = model.Blocks.OrderBy(x => x.StartDateTime).Last().EndDateTime;
			}
			else
			{
				StartDT = model.ReleaseTime;
				EndDT = model.Deadline;
			}
			ProductRework = new ProductReworkVm(model.ProductRework);
		}

		/// <summary>
		/// Gets a bindable value for ProductRework
		/// </summary>
		public ProductReworkVm ProductRework
		{
			get { return (ProductReworkVm)GetValue(ProductReworkProperty); }
			protected set { SetValue(ProductReworkProperty, value); }
		}
		public static readonly DependencyProperty ProductReworkProperty =
			DependencyProperty.Register("ProductRework", typeof(ProductReworkVm), typeof(JobListItemVm), new UIPropertyMetadata(null));
		
		/// <summary>
		/// Gets the binable count of blocks
		/// </summary>
		public int BlocksCount
		{
			get { return (int)GetValue(BlocksCountProperty); }
			protected set { SetValue(BlocksCountProperty, value); }
		}
		public static readonly DependencyProperty BlocksCountProperty =
			DependencyProperty.Register("BlocksCount", typeof(int), typeof(JobListItemVm), new UIPropertyMetadata(0));
		
		/// <summary>
		/// Gets a bindable value for starting date time of this job
		/// <para>Equal to release time if no blocks, otherwise equal to start of first block</para>
		/// </summary>
		public DateTime StartDT
		{
			get { return (DateTime)GetValue(StartDTProperty); }
			protected set { SetValue(StartDTProperty, value); }
		}
		public static readonly DependencyProperty StartDTProperty =
			DependencyProperty.Register("StartDT", typeof(DateTime), typeof(JobListItemVm), new UIPropertyMetadata(DateTime.Now));
		
		/// <summary>
		/// Gets a bindable value for ending date time of this job
		/// <para>Equal to deadline if no blocks, otherwise equal to end of last block</para>
		/// </summary>
		public DateTime EndDT
		{
			get { return (DateTime)GetValue(EndDTProperty); }
			protected set { SetValue(EndDTProperty, value); }
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
		/// <summary>
		/// Gets or sets a bindable command for selecting this item
		/// </summary>
		public Commands.Command SelectCommand
		{
			get { return (Commands.Command)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		public static readonly DependencyProperty SelectCommandProperty =
			DependencyProperty.Register("SelectCommand", typeof(Commands.Command), typeof(JobListVm), new UIPropertyMetadata(null));
		#endregion

		/// <summary>
		/// Updates the description of the model associated with this vm (database commit)
		/// </summary>
		/// <param name="val">description string</param>
		public void UpdateDescription(string val)
		{
			Model.Description = val;
			new DataServices.JobDataService().UpdateDescriptionOnly(Model);
		}
	}
}
