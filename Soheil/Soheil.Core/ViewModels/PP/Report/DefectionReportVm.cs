using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class DefectionReportVm : DependencyObject
	{
		public Model.DefectionReport Model { get; protected set; }
		readonly DefectionReportCollection Parent;

		public DefectionReportVm(DefectionReportCollection parent, Model.DefectionReport model)
		{

			Model = model;
			Index = parent.Parent.DefectionReports.List.Count + 1;
			Parent = parent;
			IsG2 = model.IsG2;

			ProductDefection = FilterBoxVm.CreateForProductDefections(
				model.ProductDefection == null ? -1 : model.ProductDefection.Id, 
				model.ProcessReport.Process.StateStationActivity.StateStation.State.FPC.Product.Id);
			var pdrepo = new Dal.Repository<Model.ProductDefection>(Parent.Parent.UOW);
			ProductDefection.FilterableItemSelected += (s, old, v) => 
				Model.ProductDefection = pdrepo.FirstOrDefault(x => x.Id == v.Id);
			if (ProductDefection.SelectedItem == null) ProductDefection.SelectedItem = ProductDefection.FilteredList.FirstOrDefault();

			//create and load OperatorDefectionReports
			GuiltyOperators = FilterBoxVmCollection.CreateForGuiltyOperators(model.OperatorDefectionReports, Parent.Parent.UOW);
			var odrRepo = new Dal.Repository<Model.OperatorDefectionReport>(Parent.Parent.UOW);
			GuiltyOperators.OperatorSelected += (vm, oldOp, newOp) =>
			{
				if (newOp.Model == null) return;

				if (vm.Model == null)
				{
					//create and add new ODR
					var odr = new Model.OperatorDefectionReport
					{
						DefectionReport = model,
						Operator = newOp.Model,
						ModifiedBy = LoginInfo.Id,
					};
					odrRepo.Add(odr);
					vm.Model = odr;
				}
				else
				{
					//update existing ODR
					(vm.Model as Model.OperatorDefectionReport).Operator = newOp.Model;
				}
			};
			GuiltyOperators.OperatorRemoved += vm =>
			{
				if (vm.Model != null)
				{
					model.OperatorDefectionReports.Remove(vm.Model as Model.OperatorDefectionReport);
					odrRepo.Delete(vm.Model as Model.OperatorDefectionReport);
				}
			};
	
			AffectsTaskReport = model.AffectsTaskReport;
			LostSeconds = model.LostTime;
			LostCount = model.LostCount;
			Description = model.Description;
			
			DeleteCommand = new Commands.Command(o => 
			{
				//correct sums
				Parent.SumOfLostCount -= LostCount;
				Parent.SumOfLostTime-= LostSeconds;
				Parent.SumOfTimeEquivalent -= TimeEquivalent;
				Parent.SumOfCountEquivalent -= QuantityEquivalent;

				//delete
				Parent.List.Remove(this);
				Model.ProcessReport.DefectionReports.Remove(Model);
				new DataServices.ProcessReportDataService(Parent.Parent.UOW).Delete(Model);

				//reset indices
				for (int i = 0; i < Parent.List.Count; i++)
				{
					Parent.List[i].Index = i + 1;
				}
			});
		}


		/// <summary>
		/// Gets or sets the bindable index of this report
		/// </summary>
		public int Index
		{
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public static readonly DependencyProperty IndexProperty =
			DependencyProperty.Register("Index", typeof(int), typeof(DefectionReportVm), new UIPropertyMetadata(0));
		/// <summary>
		/// Gets or sets a bindable value that indicates whether the count of this report affects TaskReport's output
		/// </summary>
		public bool AffectsTaskReport
		{
			get { return (bool)GetValue(AffectsTaskReportProperty); }
			set { SetValue(AffectsTaskReportProperty, value); }
		}
		public static readonly DependencyProperty AffectsTaskReportProperty =
			DependencyProperty.Register("AffectsTaskReport", typeof(bool), typeof(DefectionReportVm),
			new UIPropertyMetadata(true, (d, e) => ((DefectionReportVm)d).Model.AffectsTaskReport = (bool)e.NewValue));
		/// <summary>
		/// Gets a bindable value that indicates if the Defection of this Vm represents a Grade 2
		/// <para>If false, the the defection is (unusable) defection</para>
		/// </summary>
		public bool IsG2
		{
			get { return (bool)GetValue(IsG2Property); }
			set { SetValue(IsG2Property, value); }
		}
		public static readonly DependencyProperty IsG2Property =
			DependencyProperty.Register("IsG2", typeof(bool), typeof(DefectionReportVm), 
			new UIPropertyMetadata(false, (d, e) => ((DefectionReportVm)d).Model.IsG2 = (bool)e.NewValue));

		/// <summary>
		/// Gets or set the bindable description
		/// </summary>
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public static readonly DependencyProperty DescriptionProperty =
			DependencyProperty.Register("Description", typeof(string), typeof(DefectionReportVm),
			new UIPropertyMetadata(null, (d, e) =>
			{
				var vm = (DefectionReportVm)d;
				var val = (string)e.NewValue;
				if (val == null) return;
				vm.Model.Description = val;
			}));

		#region Time
		//LostSeconds Dependency Property
		public int LostSeconds
		{
			get { return (int)GetValue(LostSecondsProperty); }
			set { SetValue(LostSecondsProperty, value); }
		}
		public static readonly DependencyProperty LostSecondsProperty =
			DependencyProperty.Register("LostSeconds", typeof(int), typeof(DefectionReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (DefectionReportVm)d;
				var val = (int)e.NewValue;
				vm.Model.LostTime = val;
				vm.updateEquivalents(val, vm.LostCount);
				vm.Parent.SumOfLostTime += (val - (int)e.OldValue);
			}));
		//LostCount Dependency Property
		public int LostCount
		{
			get { return (int)GetValue(LostCountProperty); }
			set { SetValue(LostCountProperty, value); }
		}
		public static readonly DependencyProperty LostCountProperty =
			DependencyProperty.Register("LostCount", typeof(int), typeof(DefectionReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (DefectionReportVm)d;
				var val = (int)e.NewValue;
				vm.Model.LostCount = val;
				vm.updateEquivalents(vm.LostSeconds, val);
				vm.Parent.SumOfLostCount += (val - (int)e.OldValue);
			}));
		private void updateEquivalents(int secs, int counts)
		{
			float ct = this.Model.ProcessReport.Process.StateStationActivity.CycleTime;
			TimeEquivalent = (int)(secs + counts * ct);
			QuantityEquivalent = (int)(counts + secs / ct);
		}
		//TimeEquivalent Dependency Property
		public int TimeEquivalent
		{
			get { return (int)GetValue(TimeEquivalentProperty); }
			set { SetValue(TimeEquivalentProperty, value); }
		}
		public static readonly DependencyProperty TimeEquivalentProperty =
			DependencyProperty.Register("TimeEquivalent", typeof(int), typeof(DefectionReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				((DefectionReportVm)d).Parent.SumOfTimeEquivalent += ((int)e.NewValue - (int)e.OldValue);
			}));
		//QuantityEquivalent Dependency Property
		public int QuantityEquivalent
		{
			get { return (int)GetValue(QuantityEquivalentProperty); }
			set { SetValue(QuantityEquivalentProperty, value); }
		}
		public static readonly DependencyProperty QuantityEquivalentProperty =
			DependencyProperty.Register("QuantityEquivalent", typeof(int), typeof(DefectionReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				((DefectionReportVm)d).Parent.SumOfCountEquivalent += ((int)e.NewValue - (int)e.OldValue);
			}));
		#endregion

		//ProductDefection Dependency Property
		public FilterBoxVm ProductDefection
		{
			get { return (FilterBoxVm)GetValue(ProductDefectionProperty); }
			set { SetValue(ProductDefectionProperty, value); }
		}
		public static readonly DependencyProperty ProductDefectionProperty =
			DependencyProperty.Register("ProductDefection", typeof(FilterBoxVm), typeof(DefectionReportVm), new UIPropertyMetadata(null));
		//GuiltyOperators Dependency Property
		public FilterBoxVmCollection GuiltyOperators
		{
			get { return (FilterBoxVmCollection)GetValue(GuiltyOperatorsProperty); }
			set { SetValue(GuiltyOperatorsProperty, value); }
		}
		public static readonly DependencyProperty GuiltyOperatorsProperty =
			DependencyProperty.Register("GuiltyOperators", typeof(FilterBoxVmCollection), typeof(DefectionReportVm), new UIPropertyMetadata(null));

		//DeleteCommand Dependency Property
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(DefectionReportVm), new UIPropertyMetadata(null));
	}
}
