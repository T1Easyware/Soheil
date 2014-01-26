using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Soheil.Core.ViewModels.Fpc
{
	public class ToolboxItemVm : DragTarget
	{
		public ToolboxItemVm(FpcWindowVm parentWindowVm)
		{
			_parentWindowVm = parentWindowVm;
		}
		public override int Id { get { return ContentData.Id; } }

		FpcWindowVm _parentWindowVm;

		public StateConfigVm GetUnderlyingStateConfig(Point mouse)
		{
			var states = _parentWindowVm.States.Where(x => x.ShowDetails && x.Config != null);
			foreach (var state in states)
			{
				if (state.Config.ContentsList.Any(s => 
					s.Containment.Id == ContentData.Id && 
					!s.IsDropIndicator)) continue;
				Rect r = new Rect(state.Location.X, state.Location.Y, state.Width, state.Height);
				if (r.Contains(mouse.X, mouse.Y))
					return state.Config;
			}
			return null;
		}
		public StateStationVm GetUnderlyingStateStation(Point mouse)
		{
			var states = _parentWindowVm.States.Where(x => x.ShowDetails && x.Config != null);
			foreach (var state in states)
			{
				var station = state.Config.ContentsList.SingleOrDefault(x => x.IsExpanded) as StateStationVm;
				if (station == null) continue;
				/*if (station.ContentsList.Any(ss =>
					ss.Containment.Id == ContentData.Id && 
					!ss.IsDropIndicator)) continue;*/
				Rect r = new Rect(state.Location.X, state.Location.Y, state.Width, state.Height);
				if (r.Contains(mouse.X, mouse.Y))
					return station;
			}
			return null;
		}
		public StateStationActivityVm GetUnderlyingStateStationActivity(Point mouse)
		{
			var states = _parentWindowVm.States.Where(x => x.ShowDetails && x.Config != null);
			foreach (var state in states)
			{
				var station = state.Config.ContentsList.SingleOrDefault(x => x.IsExpanded
					&& (x.Containment as StationVm).StationMachines.Any(y => y.Machine.Id == this.ContentData.Id));
				if (station == null) continue;
				var activity = station.ContentsList.SingleOrDefault(x => x.IsExpanded) as StateStationActivityVm;
				if (activity == null) continue;
				if (activity.ContentsList.Any(ssa => 
					ssa.Containment.Id == ContentData.Id 
					&& !ssa.IsDropIndicator)) continue;
				Rect r = new Rect(state.Location.X, state.Location.Y, state.Width, state.Height);
				if (r.Contains(mouse.X, mouse.Y))
					return activity;
			}
			return null;
		}
		//Text Dependency Property
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(ToolboxItemVm), new UIPropertyMetadata(""));
		//ContentData Dependency Property
		public IToolboxData ContentData
		{
			get { return (IToolboxData)GetValue(ContentDataProperty); }
			set { SetValue(ContentDataProperty, value); }
		}
		public static readonly DependencyProperty ContentDataProperty =
			DependencyProperty.Register("ContentData", typeof(IToolboxData), typeof(ToolboxItemVm), new UIPropertyMetadata(null));
		//CanDrop Dependency Property
		public bool CanDrop
		{
			get { return (bool)GetValue(CanDropProperty); }
			set { SetValue(CanDropProperty, value); }
		}
		public static readonly DependencyProperty CanDropProperty =
			DependencyProperty.Register("CanDrop", typeof(bool), typeof(ToolboxItemVm), new UIPropertyMetadata(false));
	}
}
