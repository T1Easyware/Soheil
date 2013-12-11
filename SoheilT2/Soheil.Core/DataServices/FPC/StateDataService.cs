using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels.Fpc;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
	public class StateDataService : DataServiceBase, IDataService<State>
	{
		Repository<State> _stateRepository;
		Repository<StateStation> _stateStationRepository;
		Repository<StateStationActivity> _stateStationActivityRepository;

		/// <summary>
		/// Use this constructor only if no Fpc viewModel is involved in furthur calls
		/// </summary>
		public StateDataService()
		{
			context = new SoheilEdmContext();
			_stateRepository = new Repository<State>(context);
			_stateStationRepository = new Repository<StateStation>(context);
			_stateStationActivityRepository = new Repository<StateStationActivity>(context);
		}
		public StateDataService(SoheilEdmContext context)
		{
			this.context = context;
			_stateRepository = new Repository<State>(context);
			_stateStationRepository = new Repository<StateStation>(context);
			_stateStationActivityRepository = new Repository<StateStationActivity>(context);
		}

		public IEnumerable<State> GetStatesByFpcId(int fpcId)
		{
				return _stateRepository
					.Find(x => x.FPC.Id == fpcId,
					"OnProductRework",
					"OnProductRework.Rework",
					"FPC",
					"StateStations",
					"StateStations.Station",
					"StateStations.Station.StationMachines",
					"StateStations.Station.StationMachines.Machine",
					"StateStations.StateStationActivities", 
					"StateStations.StateStationActivities.Activity",
					"StateStations.StateStationActivities.StateStationActivityMachines", 
					"StateStations.StateStationActivities.StateStationActivityMachines.Machine"
					)
					.ToList();
		}
		public State GetStartingState(int fpcId, ProductRework productRework)
		{
			if (productRework.Rework == null)
				return _stateRepository.FirstOrDefault(x => 
						x.FPC.Id == fpcId 
						&& x.StateTypeNr == (int)StateType.Start,
					"FPC");
			return _stateRepository.FirstOrDefault(x => 
					x.FPC.Id == fpcId 
					&& x.StateTypeNr == (int)StateType.Rework 
					&& x.OnProductRework.Id == productRework.Id,
				"OnProductRework", "FPC");
		}

		public IEnumerable<StateStationActivity> GetStateStationActivities(int stationId, params string[] includePath)
		{
				if (includePath != null)
					return _stateStationActivityRepository
						.Find(x => x.StateStation.Station.Id == stationId, includePath)
						.ToList();
				else
					return _stateStationActivityRepository
						.Find(x => x.StateStation.Station.Id == stationId)
						.ToList();
		}


		#region IDataService<State> Members
		public event EventHandler<ModelAddedEventArgs<State>> StateAdded;

		public State GetSingle(int id)
		{
			return _stateRepository.FirstOrDefault(x => x.Id == id);
		}

		public ObservableCollection<State> GetAll()
		{
			return new ObservableCollection<State>(_stateRepository.GetAll());
		}

		public ObservableCollection<State> GetActives()
		{
			return new ObservableCollection<State>(_stateRepository.Find(x=>x.StateTypeNr == (int)StateType.Mid));
		}

		public int AddModel(State model)
		{
			//!@#$%^
			_stateRepository.Add(model);
			context.SaveChanges();
			return model.Id;
		}

		public void UpdateModel(State model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(State model)
		{
			throw new NotImplementedException();
		}

		public void AttachModel(State model)
		{
			throw new NotImplementedException();
		}

		#endregion

		//StateVm

		/// <summary>
		/// Adds the view model data to the model.
		/// </summary>
		/// <param name="viewModel">The view model.</param>
		public int AddModel(StateVm viewModel)
		{
			int id;
			using (var context = new SoheilEdmContext())
			{
				var fpcRepository = new Repository<FPC>(context);
				var prRepository = new Repository<ProductRework>(context);
				var state = new State
				{
					Code = viewModel.Code,
					Name = viewModel.Name,
					X = (float)viewModel.Location.X,
					Y = (float)viewModel.Location.Y,
					FPC = fpcRepository.FirstOrDefault(x => x.Id == viewModel.FPC.Id),
					StateType = viewModel.StateType,
					OnProductRework = viewModel.StateType == StateType.Mid || viewModel.StateType == StateType.Rework ?
						(viewModel.ProductRework == null
							? prRepository.FirstOrDefault(x => x.Product.Id == viewModel.FPC.Product.Id && x.Rework == null)
							: prRepository.FirstOrDefault(x => x.Id == viewModel.ProductRework.Id)
						) : null
				};
				if (viewModel.StateType == StateType.Mid)
				{
					var stationRepository = new Repository<Station>(context);
					var activityRepository = new Repository<Activity>(context);
					var machineRepository = new Repository<Machine>(context);
					viewModel.Config.ContentsList.RemoveWhere(x => x.IsDropIndicator);
					foreach (StateStationVm ss in viewModel.Config.ContentsList)
					{
						var stateStation = new StateStation
						{
							State = state,
							Station = stationRepository.FirstOrDefault(x => x.Id == ss.Containment.Id),
						};
						ss.ContentsList.RemoveWhere(x => x.IsDropIndicator);
						foreach (StateStationActivityVm ssa in ss.ContentsList)
						{
							var stateStationActivity = new StateStationActivity
							{
								StateStation = stateStation,
								ManHour = ssa.ManHour,
								CycleTime = ssa.CycleTime,
								Activity = activityRepository.FirstOrDefault(x => x.Id == ssa.Containment.Id),
								//CreatedDate
								//ModifiedBy
								//ModifiedDate
								//Status = (byte)Status.Active
							};
							ssa.ContentsList.RemoveWhere(x => x.IsDropIndicator);
							foreach (StateStationActivityMachineVm ssam in ssa.ContentsList)
							{
								stateStationActivity.StateStationActivityMachines.Add(new StateStationActivityMachine
								{
									StateStationActivity = stateStationActivity,
									Machine = machineRepository.FirstOrDefault(x => x.Id == ssam.Containment.Id),
									IsFixed = ssam.IsDefault,
								});
							}
							stateStation.StateStationActivities.Add(stateStationActivity);
						}
						state.StateStations.Add(stateStation);
					}
				}
				_stateRepository.Add(state);
				context.Commit();
				if (StateAdded != null)
					StateAdded(this, new ModelAddedEventArgs<State>(state));
				id = state.Id;
			}
			return id;
		}

		/// <summary>
		/// Updates the view model data within the model.
		/// </summary>
		/// <param name="viewModel">The view model.</param>
		public void UpdateModel(StateVm viewModel)
		{
			using (var context = new SoheilEdmContext())
			{
				//FetchConfig(viewModel);

				var sRepos = new Repository<State>(context);

				Repository<ProductRework> prRepos = new Repository<ProductRework>(context);
				Repository<Station> stationRepos = new Repository<Station>(context);
				Repository<Activity> activityRepos = new Repository<Activity>(context);
				Repository<Machine> machineRepos = new Repository<Machine>(context);

				var stateEnt = sRepos.FirstOrDefault(x => x.Id == viewModel.Id);//select
				//update state data
				stateEnt.Code = viewModel.Code;
				stateEnt.Name = viewModel.Name;
				stateEnt.OnProductRework = (viewModel.StateType == StateType.Mid || viewModel.StateType == StateType.Rework) ?
					(viewModel.ProductRework == null
						? prRepos.FirstOrDefault(x => x.Product.Id == viewModel.FPC.Product.Id && x.Rework == null)
						: prRepos.FirstOrDefault(x => x.Id == viewModel.ProductRework.Id)
					) : null;
				stateEnt.X = (float)viewModel.Location.X;
				stateEnt.Y = (float)viewModel.Location.Y;
				stateEnt.StateType = viewModel.StateType;
				//entity.CreatedDate = viewModel.CreatedDate;
				//entity.ModifiedBy = viewModel.ModifiedBy;
				//entity.ModifiedDate = viewModel.ModifiedDate;
				//entity.Status = (byte)viewModel.Status;
				#region update or add each stateStations (in current state's viewModel)
				viewModel.Config.ContentsList.RemoveWhere(x => x.IsDropIndicator);
				foreach (StateStationVm ss in viewModel.Config.ContentsList)
				{
					var ssEnt = stateEnt.StateStations.FirstOrDefault(x => x.Station.Id == ss.Containment.Id);//select
					if (ssEnt == null)//add
					{
						#region Add new StateStation
						ssEnt = new StateStation
						{
							State = stateEnt,
							Station = stationRepos.FirstOrDefault(x => x.Id == ss.Containment.Id)
						};
						stateEnt.StateStations.Add(ssEnt);
						#endregion
					}
					#region update or add each stateStationActivities (in current stateStation's viewModel)
					ss.ContentsList.RemoveWhere(x => x.IsDropIndicator);
					foreach (StateStationActivityVm ssa in ss.ContentsList)
					{
						var ssaEnt = ssEnt.StateStationActivities.FirstOrDefault(x => x.Activity.Id == ssa.Containment.Id);//select
						if (ssaEnt == null)//add
						{
							#region Add new StateStationActivity
							ssaEnt = new StateStationActivity
							{
								StateStation = ssEnt,
								Activity = activityRepos.FirstOrDefault(x => x.Id == ssa.Containment.Id),
								CycleTime = ssa.CycleTime,
								ManHour = ssa.ManHour,
							};
							ssEnt.StateStationActivities.Add(ssaEnt);
							#endregion
						}
						else
						{
							#region Update StateStationActivity
							ssaEnt.CycleTime = ssa.CycleTime;
							ssaEnt.ManHour = ssa.ManHour;
							#endregion
						}
						#region update or add each stateStationActivityMachine (in current stateStationActivity's viewModel)
						ssa.ContentsList.RemoveWhere(x => x.IsDropIndicator);
						foreach (StateStationActivityMachineVm ssam in ssa.ContentsList)
						{
							var ssamEnt = ssaEnt.StateStationActivityMachines.FirstOrDefault(x => x.Machine.Id == ssam.Containment.Id);//select
							if (ssamEnt == null)//add
							{
								#region Add new StateStationActivityMachine
								ssamEnt = new StateStationActivityMachine
								{
									StateStationActivity = ssaEnt,
									Machine = machineRepos.FirstOrDefault(x => x.Id == ssam.Containment.Id),
									IsFixed = ssam.IsDefault,
								};
								ssaEnt.StateStationActivityMachines.Add(ssamEnt);
								#endregion
							}
							else
							{
								#region Update StateStationActivityMachine
								ssamEnt.IsFixed = ssam.IsDefault;
								#endregion
							}
						}
						#endregion
					}
					#endregion
				}
				#endregion

				//delete
				foreach (StateStation ssEnt in stateEnt.StateStations.ToList())
				{
					var ss = viewModel.Config.ContentsList.FirstOrDefault(x => x.Containment.Id == ssEnt.Station.Id);
					if (ss == null)//remove ss
					{
						foreach (var ssaEnt in ssEnt.StateStationActivities.ToList())
						{
							foreach (var ssamEnt in ssaEnt.StateStationActivityMachines.ToList())
							{
								ssaEnt.StateStationActivityMachines.Remove(ssamEnt);
								new Repository<StateStationActivityMachine>(context).Delete(ssamEnt);
							}
							ssEnt.StateStationActivities.Remove(ssaEnt);
							_stateStationActivityRepository.Delete(ssaEnt);
						}
						stateEnt.StateStations.Remove(ssEnt);
						_stateStationRepository.Delete(ssEnt);
					}
					else//search ss.ssa
					{
						foreach (StateStationActivity ssaEnt in ssEnt.StateStationActivities.ToList())
						{
							var ssa = ss.ContentsList.FirstOrDefault(x => x.Containment.Id == ssaEnt.Activity.Id);
							if (ssa == null)//remove ss.ssa
							{
								foreach (var ssamEnt in ssaEnt.StateStationActivityMachines.ToList())
								{
									ssaEnt.StateStationActivityMachines.Remove(ssamEnt);
									new Repository<StateStationActivityMachine>(context).Delete(ssamEnt);
								}
								ssEnt.StateStationActivities.Remove(ssaEnt);
								_stateStationActivityRepository.Delete(ssaEnt);
							}
							else//search ss.ssa.ssam
							{
								foreach (StateStationActivityMachine ssamEnt in ssaEnt.StateStationActivityMachines.ToList())
								{
									var ssam = ssa.ContentsList.FirstOrDefault(x => x.Containment.Id == ssamEnt.Machine.Id);
									if (ssam == null)//remove ss.ssa.ssam
									{
										ssaEnt.StateStationActivityMachines.Remove(ssamEnt);
										new Repository<StateStationActivityMachine>(context).Delete(ssamEnt);
									}
								}
							}
						}
					}
				}
				context.Commit();
				//id
				foreach (var ss in viewModel.Config.ContentsList)
				{
					var ssEnt = stateEnt.StateStations.First(x => x.Station.Id == ss.Containment.Id);
					ss.Id = ssEnt.Id;
					foreach (var ssa in ss.ContentsList)
					{
						var ssaEnt = ssEnt.StateStationActivities.First(x => x.Activity.Id == ssa.Containment.Id);
						ssa.Id = ssaEnt.Id;
						foreach (var ssam in ssa.ContentsList)
						{
							var ssamEnt = ssaEnt.StateStationActivityMachines.First(x => x.Machine.Id == ssam.Containment.Id);
							ssam.Id = ssamEnt.Id;
						}
					}
				}
			}
		}

		/// <summary>
		/// Deletes view model data from the model.
		/// </summary>
		/// <param name="viewModel">The view model.</param>
		public void DeleteModel(StateVm viewModel)
		{
			using (var context = new Soheil.Dal.SoheilEdmContext())
			{
				Repository<State> repository = new Repository<State>(context);
				var obj = repository.FirstOrDefault(x => x.Id == viewModel.Id);
				if (obj != null)
				{
					foreach (var ss in obj.StateStations.ToList())
					{
						foreach (var ssa in ss.StateStationActivities.ToList())
						{
							foreach (var ssam in ssa.StateStationActivityMachines.ToList())
							{
								ssa.StateStationActivityMachines.Remove(ssam);
							}
							ss.StateStationActivities.Remove(ssa);
						}
						obj.StateStations.Remove(ss);
					}
					var fpc = obj.FPC;
					repository.Delete(obj);
					fpc.States.Remove(obj);
					Repository<Connector> connectorRepository = new Repository<Connector>(context);
					var connectors = connectorRepository.Find(x => x.StartState.Id == obj.Id || x.EndState.Id == obj.Id);
					foreach (var connector in connectors)
					{
						connectorRepository.Delete(connector);
					}
				}
				context.Commit();
			}
		}

		/// <summary>
		/// If exists, updates current model otherwise adds the new model to the database.
		/// </summary>
		/// <param name="viewModel">The view model.</param>
		public void AttachModel(StateVm viewModel)
		{
			if (_stateRepository.Exists(s => s.Id == viewModel.Id))
			{
				UpdateModel(viewModel);
			}
			else
			{
				viewModel.Id = AddModel(viewModel);
			}
		}

		public State Clone(State model)
		{
			var clone = new State();
			clone.Code = model.Code;
			clone.Name = model.Name;
			clone.StateType = model.StateType;
			clone.X = model.X;
			clone.Y = model.Y;
			clone.OnProductRework = model.OnProductRework;
			foreach (var stateStations in model.StateStations.ToArray())
			{
				clone.StateStations.Add(new StateStation
				{
					//State = new //!@#$%
				});
			}
			return clone;
		}
	}
}