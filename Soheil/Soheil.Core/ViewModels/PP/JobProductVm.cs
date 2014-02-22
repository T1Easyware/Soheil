using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class JobProductVm : ProductVm
	{
		/// <summary>
		/// can be used in JobEditor
		/// </summary>
		/// <param name="model"></param>
		/// <param name="parentVm"></param>
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
		//CreateNewJob Dependency Property
		public Commands.Command CreateNewJob
		{
			get { return (Commands.Command)GetValue(CreateNewJobProperty); }
			set { SetValue(CreateNewJobProperty, value); }
		}
		public static readonly DependencyProperty CreateNewJobProperty =
			DependencyProperty.Register("CreateNewJob", typeof(Commands.Command), typeof(JobProductVm), new UIPropertyMetadata(null));
	}
}