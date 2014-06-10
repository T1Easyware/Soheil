using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP.Report
{
	public class StoppageReportVm : DependencyObject
	{
		public Model.StoppageReport Model { get; protected set; }
		readonly StoppageReportCollection Parent;

		public StoppageReportVm(StoppageReportCollection parent, Model.StoppageReport model)
		{
			Model = model;
			Index = parent.Parent.StoppageReports.List.Count + 1;
			Parent = parent;
			
			int[] causeIds = null;
			if (model != null)
				if (model.Cause != null)
					if (model.Cause.Parent != null)
						if (model.Cause.Parent.Parent != null)
							causeIds = new int[3] { model.Cause.Parent.Parent.Id, model.Cause.Parent.Id, model.Cause.Id };
			StoppageLevels = FilterBoxVmCollection.CreateForStoppageReport(this, causeIds);

			GuiltyOperators = FilterBoxVmCollection.CreateForGuiltyOperators(model.OperatorStoppageReports, Parent.Parent.UOW);
			var osrRepo = new Dal.Repository<Model.OperatorStoppageReport>(Parent.Parent.UOW);
			GuiltyOperators.OperatorSelected += (vm, oldOp, newOp) =>
			{
				if (newOp.Model == null) return;
				//update model
				if (vm.Model == null)
				{
					//create and add new OSR
					var osr = new Model.OperatorStoppageReport
					{
						StoppageReport = model,
						Operator = newOp.Model,
						ModifiedBy = LoginInfo.Id,
					};
					osrRepo.Add(osr);
					vm.Model = osr;
				}
				else
				{
					//update existing OSR
					vm.Model.Operator = newOp.Model;
				}
			};
			GuiltyOperators.OperatorRemoved += vm =>
			{
				if (vm.Model != null)
				{
					model.OperatorStoppageReports.Remove(vm.Model);
					osrRepo.Delete(vm.Model);
				}
			};

			AffectsTaskReport = model.AffectsTaskReport;
			LostSeconds = model.LostTime;
			LostCount = model.LostCount;
			DeleteCommand = new Commands.Command(o => 
			{
				//correct sums
				Parent.SumOfLostCount -= LostCount;
				Parent.SumOfLostTime-= LostSeconds;
				Parent.SumOfTimeEquivalent -= TimeEquivalent;
				Parent.SumOfCountEquivalent -= QuantityEquivalent;
				
				//delete
				Parent.List.Remove(this);
				Model.ProcessReport.StoppageReports.Remove(Model);
				if (Model.Id > 0) new Dal.Repository<Model.StoppageReport>(Parent.Parent.UOW).Delete(Model);

				//reset indices
				for (int i = 0; i < Parent.List.Count; i++)
				{
					Parent.List[i].Index = i + 1;
				}
			});
		}

		internal void SelectCause(int causeId)
		{
			Model.Cause = new Dal.Repository<Model.Cause>(Parent.Parent.UOW).FirstOrDefault(x => x.Id == causeId);
		}

		//Index Dependency Property
		public int Index
		{
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public static readonly DependencyProperty IndexProperty =
			DependencyProperty.Register("Index", typeof(int), typeof(StoppageReportVm), new UIPropertyMetadata(0));
		
		//AffectsTaskReport Dependency Property
		public bool AffectsTaskReport
		{
			get { return (bool)GetValue(AffectsTaskReportProperty); }
			set { SetValue(AffectsTaskReportProperty, value); }
		}
		public static readonly DependencyProperty AffectsTaskReportProperty =
			DependencyProperty.Register("AffectsTaskReport", typeof(bool), typeof(StoppageReportVm),
			new UIPropertyMetadata(true, (d, e) => ((StoppageReportVm)d).Model.AffectsTaskReport = (bool)e.NewValue));

		//SelectedCode Dependency Property
		public string SelectedCode
		{
			get { return (string)GetValue(SelectedCodeProperty); }
			set { SetValue(SelectedCodeProperty, value); }
		}
		public static readonly DependencyProperty SelectedCodeProperty =
			DependencyProperty.Register("SelectedCode", typeof(string), typeof(StoppageReportVm),
			new UIPropertyMetadata(null));

		#region Time
		//LostSeconds Dependency Property
		public int LostSeconds
		{
			get { return (int)GetValue(LostSecondsProperty); }
			set { SetValue(LostSecondsProperty, value); }
		}
		public static readonly DependencyProperty LostSecondsProperty =
			DependencyProperty.Register("LostSeconds", typeof(int), typeof(StoppageReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (StoppageReportVm)d;
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
			DependencyProperty.Register("LostCount", typeof(int), typeof(StoppageReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				var vm = (StoppageReportVm)d;
				var val = (int)e.NewValue;
				vm.Model.LostCount = val;
				vm.updateEquivalents(vm.LostSeconds, val);
				vm.Parent.SumOfLostCount += (val - (int)e.OldValue);
			}));
		private void updateEquivalents(int secs, int counts)
		{
			float ct = Model.ProcessReport.Process.StateStationActivity.CycleTime;
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
			DependencyProperty.Register("TimeEquivalent", typeof(int), typeof(StoppageReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				((StoppageReportVm)d).Parent.SumOfTimeEquivalent += ((int)e.NewValue - (int)e.OldValue);
			}));
		//QuantityEquivalent Dependency Property
		public int QuantityEquivalent
		{
			get { return (int)GetValue(QuantityEquivalentProperty); }
			set { SetValue(QuantityEquivalentProperty, value); }
		}
		public static readonly DependencyProperty QuantityEquivalentProperty =
			DependencyProperty.Register("QuantityEquivalent", typeof(int), typeof(StoppageReportVm),
			new UIPropertyMetadata(0, (d, e) =>
			{
				((StoppageReportVm)d).Parent.SumOfCountEquivalent += ((int)e.NewValue - (int)e.OldValue);
			}));
		#endregion

		//StoppageLevels Dependency Property
		public FilterBoxVmCollection StoppageLevels
		{
			get { return (FilterBoxVmCollection)GetValue(StoppageLevelsProperty); }
			set { SetValue(StoppageLevelsProperty, value); }
		}
		public static readonly DependencyProperty StoppageLevelsProperty =
			DependencyProperty.Register("StoppageLevels", typeof(FilterBoxVmCollection), typeof(StoppageReportVm), new UIPropertyMetadata(null));
		
		//GuiltyOperators Dependency Property
		public FilterBoxVmCollection GuiltyOperators
		{
			get { return (FilterBoxVmCollection)GetValue(GuiltyOperatorsProperty); }
			set { SetValue(GuiltyOperatorsProperty, value); }
		}
		public static readonly DependencyProperty GuiltyOperatorsProperty =
			DependencyProperty.Register("GuiltyOperators", typeof(FilterBoxVmCollection), typeof(StoppageReportVm), new UIPropertyMetadata(null));

		//DeleteCommand Dependency Property
		public Commands.Command DeleteCommand
		{
			get { return (Commands.Command)GetValue(DeleteCommandProperty); }
			set { SetValue(DeleteCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteCommandProperty =
			DependencyProperty.Register("DeleteCommand", typeof(Commands.Command), typeof(StoppageReportVm), new UIPropertyMetadata(null));


	}
}
