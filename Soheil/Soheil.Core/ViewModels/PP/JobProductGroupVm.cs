using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class JobProductGroupVm : ProductGroupVm
	{
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
