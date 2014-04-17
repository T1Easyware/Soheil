using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	/// <summary>
	/// ViewModel for product that can be used in JobEditor
	/// <para>Can create a new job based on the associating product and add it to a JobListVm</para>
	/// </summary>
	public class JobProductVm : ProductVm
	{
		/// <summary>
		/// Creates an instance of JobProductVm with given model, parent and data service
		/// </summary>
		/// <param name="model"></param>
		/// <param name="parentVm"></param>
		/// <param name="jobDataService"></param>
		public JobProductVm(Model.Product model, ProductGroupVm parentVm, DataServices.JobDataService jobDataService)
			: base(model, parentVm)
		{
			CreateNewJob = new Commands.Command
				(vm =>
					{
						var job = Soheil.Core.ViewModels.PP.Editor.PPEditorJob.CreateForProduct(model, jobDataService);
						((Soheil.Core.ViewModels.PP.Editor.PPJobEditorVm)vm).JobList.Add(job);
					}
				);
		}
		
		/// <summary>
		/// Gets or sets a bindable command to Create a new job from this vm
		/// <para>Command parameter must reference a valid instance of <see cref="JobListVm"/></para>
		/// </summary>
		public Commands.Command CreateNewJob
		{
			get { return (Commands.Command)GetValue(CreateNewJobProperty); }
			set { SetValue(CreateNewJobProperty, value); }
		}
		public static readonly DependencyProperty CreateNewJobProperty =
			DependencyProperty.Register("CreateNewJob", typeof(Commands.Command), typeof(JobProductVm), new UIPropertyMetadata(null));
	}
}