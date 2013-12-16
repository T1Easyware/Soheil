using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Editor
{
	public class PPEditorJob : DependencyObject
	{
		DataServices.JobDataService _jobDS;
		public PPEditorJob(ProductVm product)
		{
			Replications.Add(new Model.Job());
			_jobDS = new DataServices.JobDataService();
			FpcId = _jobDS.GetFpcIdForProductId(product.Id);
			Product = product;
			//var mainP = new DataServices.ProductDataService().GetReworks(product.Id).FirstOrDefault(x => x.Rework.Id == -1);
			ProductRework = product.ProductReworks.FirstOrDefault(x => x.Rework.Id == -1);
		}
		public PPEditorJob(int jobId)
		{
			_jobDS = new DataServices.JobDataService();
			var model = _jobDS.GetSingle(jobId);
			Replications.Add(model);

			Deadline = model.Deadline;
			ReleaseDT = model.ReleaseTime;
			Code = model.Code;
			Quantity = model.Quantity;
			Weight = model.Weight;
			Description = model.Description;
			FpcId = model.FPC.Id;
			Product = new ProductVm(model.ProductRework.Product, null);
			ProductRework = new ProductReworkVm(model.ProductRework, Product);
		}
		public int FpcId { get; set; }

		#region Basic DpProps
		//Code Dependency Property
		public string Code
		{
			get { return (string)GetValue(CodeProperty); }
			set { SetValue(CodeProperty, value); }
		}
		public static readonly DependencyProperty CodeProperty =
			DependencyProperty.Register("Code", typeof(string), typeof(PPEditorJob), new UIPropertyMetadata(null));
		//Quantity Dependency Property
		public int Quantity
		{
			get { return (int)GetValue(QuantityProperty); }
			set { SetValue(QuantityProperty, value); }
		}
		public static readonly DependencyProperty QuantityProperty =
			DependencyProperty.Register("Quantity", typeof(int), typeof(PPEditorJob), new UIPropertyMetadata(0));
		//Description Dependency Property
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public static readonly DependencyProperty DescriptionProperty =
			DependencyProperty.Register("Description", typeof(string), typeof(PPEditorJob), new UIPropertyMetadata(null));

		//Product Dependency Property
		public ProductVm Product
		{
			get { return (ProductVm)GetValue(ProductProperty); }
			set { SetValue(ProductProperty, value); }
		}
		public static readonly DependencyProperty ProductProperty =
			DependencyProperty.Register("Product", typeof(ProductVm), typeof(PPEditorJob), new UIPropertyMetadata(null));
		//ProductRework Dependency Property
		public ProductReworkVm ProductRework
		{
			get { return (ProductReworkVm)GetValue(ProductReworkProperty); }
			set { SetValue(ProductReworkProperty, value); }
		}
		public static readonly DependencyProperty ProductReworkProperty =
			DependencyProperty.Register("ProductRework", typeof(ProductReworkVm), typeof(PPEditorJob), new UIPropertyMetadata(null));
		//Weight Dependency Property
		public float Weight
		{
			get { return (float)GetValue(WeightProperty); }
			set { SetValue(WeightProperty, value); }
		}
		public static readonly DependencyProperty WeightProperty =
			DependencyProperty.Register("Weight", typeof(float), typeof(PPEditorJob),
			new UIPropertyMetadata(1f, (d, e) => { }, (d, v) =>
			{
				if ((float)v <= 0.1f) return 0.1f;
				return v;
			}));
		#endregion

		#region Deadline and ReleaseDT Date and Time
		//Deadline
		public DateTime Deadline
		{
			get { return DeadlineDate.Add(DeadlineTime); }
			set { DeadlineDate = value.Date; DeadlineTime = value.TimeOfDay; }
		}
		//Release DP
		public DateTime ReleaseDT
		{
			get { return ReleaseDate.Add(ReleaseTime); }
			set { ReleaseDate = ReleaseDT.Date; ReleaseTime = ReleaseDT.TimeOfDay; }
		}
		//DeadlineDate DP
		public DateTime DeadlineDate
		{
			get { return (DateTime)GetValue(DeadlineDateProperty); }
			set { SetValue(DeadlineDateProperty, value); }
		}
		public static readonly DependencyProperty DeadlineDateProperty =
			DependencyProperty.Register("DeadlineDate", typeof(DateTime), typeof(PPEditorJob),
			new PropertyMetadata(DateTime.Now.Date.AddMonths(1)));
		//DeadlineTime DP
		public TimeSpan DeadlineTime
		{
			get { return (TimeSpan)GetValue(DeadlineTimeProperty); }
			set { SetValue(DeadlineTimeProperty, value); }
		}
		public static readonly DependencyProperty DeadlineTimeProperty =
			DependencyProperty.Register("DeadlineTime", typeof(TimeSpan), typeof(PPEditorJob),
			new PropertyMetadata(DateTime.Now.TimeOfDay));
		//ReleaseDate DP
		public DateTime ReleaseDate
		{
			get { return (DateTime)GetValue(ReleaseDateProperty); }
			set { SetValue(ReleaseDateProperty, value); }
		}
		public static readonly DependencyProperty ReleaseDateProperty =
			DependencyProperty.Register("ReleaseDate", typeof(DateTime), typeof(PPEditorJob),
			new PropertyMetadata(DateTime.Now.Date));
		//ReleaseTime DP
		public TimeSpan ReleaseTime
		{
			get { return (TimeSpan)GetValue(ReleaseTimeProperty); }
			set { SetValue(ReleaseTimeProperty, value); }
		}
		public static readonly DependencyProperty ReleaseTimeProperty =
			DependencyProperty.Register("ReleaseTime", typeof(TimeSpan), typeof(PPEditorJob),
			new PropertyMetadata(DateTime.Now.TimeOfDay));
		#endregion

		#region Lag
		//LagSeconds Dependency Property
		public int LagSeconds
		{
			get { return (int)GetValue(LagSecondsProperty); }
			set { SetValue(LagSecondsProperty, value); }
		}
		public static readonly DependencyProperty LagSecondsProperty =
			DependencyProperty.Register("LagSeconds", typeof(int), typeof(PPEditorJob),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (PPEditorJob)d;
				var val = (int)e.NewValue;
				if (val > 0) vm.LagCount = 0;
			}));
		//LagCount Dependency Property
		public int LagCount
		{
			get { return (int)GetValue(LagCountProperty); }
			set { SetValue(LagCountProperty, value); }
		}
		public static readonly DependencyProperty LagCountProperty =
			DependencyProperty.Register("LagCount", typeof(int), typeof(PPEditorJob),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (PPEditorJob)d;
				var val = (int)e.NewValue;
				if (val > 0) vm.LagSeconds = 0;
			}));
		#endregion

		//Replications         important: only allowed to use Id
		private ObservableCollection<Model.Job> _replications = new ObservableCollection<Model.Job>();
		public ObservableCollection<Model.Job> Replications { get { return _replications; } }
		public void AddReplication()
		{
			Replications.Add(new Model.Job());
		}
		public void RemoveReplication()
		{
			if (Replications.Count > 1)
				Replications.Remove(Replications.Last());
		}
	}
}
