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
			ProductDefection = FilterBoxVm.CreateForProductDefections(
				model.ProductDefection == null ? -1 : model.ProductDefection.Id, 
				model.ProcessReport.Process.StateStationActivity.StateStation.State.FPC.Product.Id);
			GuiltyOperators = FilterBoxVmCollection.CreateForGuiltyOperators(model.OperatorDefectionReports.Select(x=>x.Operator.Id));
			AffectsTaskReport = model.AffectsTaskReport;
			LostSeconds = model.LostTime;
			LostCount = model.LostCount;
			DeleteCommand = new Commands.Command(o => 
			{
				Parent.SumOfLostCount -= LostCount;
				Parent.SumOfLostTime-= LostSeconds;
				Parent.SumOfTimeEquivalent -= TimeEquivalent;
				Parent.SumOfCountEquivalent -= QuantityEquivalent;
				Parent.List.Remove(this);
				for (int i = 0; i < Parent.List.Count; i++)
				{
					Parent.List[i].Index = i + 1;
				}
			});
		}


		//Index Dependency Property
		public int Index
		{
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public static readonly DependencyProperty IndexProperty =
			DependencyProperty.Register("Index", typeof(int), typeof(DefectionReportVm), new UIPropertyMetadata(0));
		//AffectsTaskReport Dependency Property
		public bool AffectsTaskReport
		{
			get { return (bool)GetValue(AffectsTaskReportProperty); }
			set { SetValue(AffectsTaskReportProperty, value); }
		}
		public static readonly DependencyProperty AffectsTaskReportProperty =
			DependencyProperty.Register("AffectsTaskReport", typeof(bool), typeof(DefectionReportVm), new UIPropertyMetadata(true));


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
