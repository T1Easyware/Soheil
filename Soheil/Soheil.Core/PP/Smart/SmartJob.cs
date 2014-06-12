using Soheil.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Soheil.Common.SoheilException;

namespace Soheil.Core.PP.Smart
{
	internal class SmartJob
	{
		#region Ctor
		protected SmartJob()
		{
			Layers = new List<SmartLayer>();
			_fpcDs = new DataServices.FPCDataService();
		}
		internal SmartJob(SmartManager manager, ViewModels.PP.Editor.PPEditorJob jobVm)
		{
			Manager = manager;
			Layers = new List<SmartLayer>();
			_fpcDs = new DataServices.FPCDataService();
			_fpcModel = _fpcDs.GetSingleWithStates(jobVm.FpcId);

			LagCount = jobVm.LagCount;
			LagSeconds = jobVm.LagSeconds;
			Code = jobVm.Code;
			Quantity = jobVm.Quantity;
			ReleaseTime = jobVm.ReleaseDT;
			Deadline = jobVm.Deadline;
			if (ReleaseTime < DateTime.Now) ReleaseTime = DateTime.Now;

			#region Find StartState
			State startState = null;
			bool isJobRework = true;
			if (jobVm.ProductRework.Rework == null) isJobRework = false;
			else if (jobVm.ProductRework.Rework.Id < 1) isJobRework = false;
			if (isJobRework)
			{
				startState = _fpcModel.GetStartingReworkState(jobVm.ProductRework.Id);
				if (startState == null)
					throw new Exception("Rework is not included in FPC yet.\nFirst fully define FPC.");//???
			}
			else
			{
				startState = _fpcModel.StartingState;
				if (startState == null)
					throw new Exception(string.Format(
						"FPC for {0} is not defined yet.\nFirst define FPC or choose another FPC.",jobVm.Product.Name));//???
			}
			#endregion

			if (!buildLayers(startState)) throw new Exception(string.Format(
				"Cannot trace FPC for {0}.\n FPC contains too many layers or it has a loop.", jobVm.Product.Name));//???
		}
		/// <summary>
		/// Must save FPC before using this, also this always throws
		/// </summary>
		/// <param name="fpcId"></param>
		public static void AutoRouteCheck(int fpcId)
		{
			var sj = new SmartJob();
			sj._fpcModel = sj._fpcDs.GetSingleWithStates(fpcId);
			var stateDs = new DataServices.StateDataService();
			var fpcPRs = sj._fpcDs.GetProductReworks(sj._fpcModel, true);
			List<string> errors = new List<string>();
			bool shouldWarnUser = false;
			foreach (var pr in fpcPRs)
			{
				try
				{
					var state = stateDs.GetStartingState(fpcId, pr);
					if (state == null)
						errors.Add(string.Format("Rework {0} not found in {1}", pr.Rework.Name, pr.Product.Name));
					else if (!sj.buildLayers(state))
					{
						shouldWarnUser = true;
						throw new Exception(string.Format(
							"Cannot trace FPC for {0}.\n FPC contains too many layers or it has a loop.", pr.Product.Name));//???
					}
				}
				catch (SoheilExceptionBase exp)
				{
					errors.Add(exp.Message);
					if (exp.Level == ExceptionLevel.Warning) shouldWarnUser = true;
				}
				catch (Exception exp) { errors.Add(exp.Message); }
			}
			if (errors.Any())
			{
				string msg = string.Format("{0} States have errors:\n", errors.Count);
				foreach (var err in errors)
				{
					msg = string.Format("{0}\n{1}", msg, err);
				}
				throw new SoheilExceptionBase(
					msg,
					shouldWarnUser ? ExceptionLevel.Warning : ExceptionLevel.Info,
					"مسیریابی خودکار FPC");
			}
			throw new SoheilExceptionBase("ذخیره و مسیریابی با موفقیت انجام شد", ExceptionLevel.Info, "ذخیره FPC");
		}
		#endregion

		#region Trace the Graph
		int currentLayerIndex = 0;
		const int LIMIT_numberOfLayers = 20;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="startState">Must fill OnProductRework</param>
		/// <returns></returns>
		bool buildLayers(State startState)
		{
			var currentLayer = new SmartLayer(new SmartStep(this, startState, null));
			currentLayerIndex = 0;
			return makeNextLayer(currentLayer, startState);
		}
		bool makeNextLayer(SmartLayer currentLayer, State veryStartState)
		{
			SmartLayer nextLayer = new SmartLayer();
			foreach (var step in currentLayer)
			{
				//Find destination states from current step's state
				var endStates = _fpcDs.GetDestinationStates(step.State.Id);
				//add them to nextLayer (after converting them to SmartSteps)
				nextLayer.AddRange(endStates.Select(state => new SmartStep(this, state, step)));
			}
			if (nextLayer.Count == 0)
			{
				if (currentLayer.Count == 1)
					if (currentLayer.Single().State.StateType == Common.StateType.Final)
					{
						Layers.Remove(currentLayer);
						return true;
					}

				if (veryStartState.OnProductRework == null
					|| veryStartState.OnProductRework.Rework == null)
				{
					throw new SoheilExceptionBase(
						string.Format("FPC {0} در مسیر تولید اصلی به مرحله نهایی ختم نمی شود", _fpcModel.Product.Name),
						ExceptionLevel.Warning, "مسیریابی خودکار FPC");
				}
				else if (veryStartState.OutConnectors.Any())
				{
					throw new SoheilExceptionBase(
						string.Format(
							"FPC {0} پس از گذر از {1} به مرحله نهایی ختم نمی شود",
							_fpcModel.Product.Name,
							veryStartState.OnProductRework.Name),
						ExceptionLevel.Warning, "مسیریابی خودکار FPC");
				}
				return true;
			}

			//Remove duplicate steps (present in nextLayer) from nextLayer
			nextLayer.RemoveDuplicateSteps();
			//Remove duplicate steps (present in nextLayer) from all previous layers
			nextLayer.RemoveDuplicateStepsFrom(Layers);
			//Add nextLayer to Layers
			Layers.Add(nextLayer);
			//preform the next layer search
			currentLayerIndex++;
			if (currentLayerIndex == LIMIT_numberOfLayers) return false;
			return makeNextLayer(nextLayer, veryStartState);
		}
		#endregion

		internal Block MakeBlockFrom(SmartStep step, Job jobModel)
		{
			if (!step.HasStateStation)
				throw new Exception("FPC is incomplete. Some states do not have stations.");//???

			int durationSeconds = (int)Math.Ceiling(step.BestStateStation.StateStationActivities
					.Max(x => x.CycleTime * Quantity));
			var block = new Block
			{
				Job = jobModel,
				Code = jobModel.Code + step.State.Code,
				StartDateTime = step.ActualReleaseTime,
				StateStation = step.BestStateStation,
				DurationSeconds = durationSeconds,
				EndDateTime = step.ActualReleaseTime.AddSeconds(durationSeconds),
				BlockTargetPoint = jobModel.Quantity
			};
			var task = new Task
			{
				Block = block,
				DurationSeconds = block.DurationSeconds,
				EndDateTime = block.EndDateTime,
				StartDateTime = block.StartDateTime,
				TaskTargetPoint = block.BlockTargetPoint,
			};
			foreach (var ssa in step.BestStateStation.StateStationActivities)
			{
				var process = new Process
				{
					Task = task,
					Code = block.Code + ssa.Activity.Code,
					TargetCount = Quantity,
					StateStationActivity = ssa,
				};
				foreach (var ssam in ssa.StateStationActivityMachines)
				{
					if (ssam.IsFixed)
						process.SelectedMachines.Add(new SelectedMachine
						{
							Process = process,
							StateStationActivityMachine = ssam
						});
				}
				task.Processes.Add(process);
			}
			return block;
		}

		private DataServices.FPCDataService _fpcDs;
		internal SmartManager Manager { get; private set; }
		private FPC _fpcModel;

		internal List<SmartLayer> Layers { get; private set; }

		internal int LagCount { get; private set; }
		internal int LagSeconds { get; private set; }

		internal string Code { get; private set; }
		internal int Quantity { get; private set; }
		internal DateTime ReleaseTime { get; private set; }
		internal DateTime Deadline { get; private set; }
	}
}
