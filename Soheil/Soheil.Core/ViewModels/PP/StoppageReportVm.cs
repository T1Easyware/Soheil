using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Soheil.Core.ViewModels.PP
{
	public class StoppageReportVm : DependencyObject
	{
		public StoppageReportVm(StoppageReportCollection parent, Model.StoppageReport model)
		{
			Index = parent.Parent.StoppageReports.List.Count + 1;
			Parent = parent;
			int[] causeIds = null;
			if (model != null)
				if (model.Cause != null)
					if (model.Cause.Parent != null)
						if (model.Cause.Parent.Parent != null)
							causeIds = new int[3] { model.Cause.Parent.Parent.Id, model.Cause.Parent.Id, model.Cause.Id };
			StoppageLevels = FilterBoxVmCollection.CreateForStoppageReport(this, causeIds);
			GuiltyOperators = FilterBoxVmCollection.CreateForGuiltyOperators(
				model == null ? null : model.OperatorStoppageReports.Select(x => x.Operator.Id));
			if (model != null)
			{
				AffectsTaskReport = model.AffectsTaskReport;
				Id = model.Id;
				LostSeconds = model.LostTime;
				LostCount = model.LostCount;
			}
			else Id = -1;
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

		/// <summary>
		/// StoppageReport Id
		/// </summary>
		public int Id { get; set; }
		public StoppageReportCollection Parent { get; set; }

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
			DependencyProperty.Register("AffectsTaskReport", typeof(bool), typeof(StoppageReportVm), new UIPropertyMetadata(true));

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
				vm.updateEquivalents(vm.LostSeconds, val);
				vm.Parent.SumOfLostCount += (val - (int)e.OldValue);
			}));
		private void updateEquivalents(int secs, int counts)
		{
			float ct = Parent.Parent.Parent.Process.StateStationActivity.CycleTime;
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
