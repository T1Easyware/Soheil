using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	/// <summary>
	/// View model of product group used in job
	/// <para><see cref="JobProductVm"/> instances in this vm can create new jobs based on their associating products and add them to a JobListVm</para>
	/// </summary>
	public class JobProductGroupVm : ProductGroupVm
	{
		/// <summary>
		/// Creates an instance of this vm with given model and data service
		/// </summary>
		/// <param name="model">products and product reworks of this model are also in use</param>
		/// <param name="jobDataService"></param>
		public JobProductGroupVm(Model.ProductGroup model, DataServices.JobDataService jobDataService)
		{
			if (model == null) return;
			_id = model.Id;
			Name = model.Name;
			Code = model.Code;

			foreach (var p_model in model.Products)
			{
				var p = new JobProductVm(p_model, this, jobDataService);
				Products.Add(p);
			}

		}
	}
}
