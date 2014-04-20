using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels.Fpc;
using Soheil.Dal;
using Soheil.Model;
using Soheil.Core.PP.Smart;
using Soheil.Common;

namespace Soheil.Core.DataServices
{
	public class FPCDataService : DataServiceBase, IDataService<FPC>
	{
		public event EventHandler<ModelAddedEventArgs<FPC>> FpcAdded;

		Repository<FPC> _fpcRepository;

		internal StateDataService stateDataService { get; private set; }
		internal ConnectorDataService connectorDataService { get; private set; }
		internal StationDataService stationDataService { get; private set; }
		internal ActivityGroupDataService activityGroupDataService { get; private set; }
		internal MachineFamilyDataService machineFamilyDataService { get; private set; }

		public FPCDataService()
			: this(new SoheilEdmContext())
		{
		}
		internal FPCDataService(SoheilEdmContext context)
		{
			this.context = context;
			_fpcRepository = new Repository<FPC>(context);

			//other dataservices
			stateDataService = new StateDataService(context);
			connectorDataService = new ConnectorDataService(context, this);
			stationDataService = new StationDataService(context);
			activityGroupDataService = new ActivityGroupDataService(context);
			machineFamilyDataService = new MachineFamilyDataService(context);
		}

		public IEnumerable<FPC> GetAllForProduct(int productId)
		{
			var list = new List<FPC>();
			var models = _fpcRepository.Find(x => x.Product.Id == productId, "Product");
			foreach (var item in models)
			{
				list.Add(item);
			}
			return list;
		}

		public FPC GetActiveForProduct(int productId)
		{
			var model = _fpcRepository.FirstOrDefault(x => x.Product.Id == productId && x.IsDefault, "Product");
			return model;
		}
		/// <summary>
		/// Save all changes to this context
		/// </summary>
		public void ApplyChanges()
		{
			context.Commit();
		}

		public FPC GetSingleWithStates(int id)
		{
			return _fpcRepository.FirstOrDefault(x => x.Id == id, "Product", "States", "States.OnProductRework");
		}

		#region IDataService
		public FPC GetSingle(int id)
		{
			return _fpcRepository.FirstOrDefault(x => x.Id == id, "Product");
		}

		public ObservableCollection<FPC> GetAll()
		{
			var models = _fpcRepository.GetAll("Product");
			return new ObservableCollection<FPC>(models);
		}

		public ObservableCollection<FPC> GetActives()
		{
			var models = _fpcRepository.Find(x => x.IsDefault, "Product");
			return new ObservableCollection<FPC>(models);
		}

		public int AddModel(FPC model)
		{
			throw new NotImplementedException();
		}

		public int AddModel(FPC model, int groupId)
		{
			var product = new Repository<Product>(context).Single(group => group.Id == groupId);
			product.FPCs.Add(model);
			model.Product = product;
			model.CreatedDate = DateTime.Now;
			context.Commit();

			if (FpcAdded != null)
				FpcAdded(this, new ModelAddedEventArgs<FPC>(model));
			return model.Id;
		}

		public void UpdateModel(FPC model)
		{
			throw new NotImplementedException();
		}

		public void DeleteModel(FPC model)
		{
			throw new NotImplementedException();
		}

		public void AttachModel(FPC model)
		{
			model.ModifiedDate = DateTime.Now;
			model.ModifiedBy = LoginInfo.Id;
			context.SaveChanges();
		} 
		#endregion


		/// <summary>
		/// creates and saves a clone of the given model (id) and returns it
		/// </summary>
		/// <param name="id">Id of source model</param>
		/// <returns>clone</returns>
		internal FPC CloneModelById(int fpcId)
		{
			var model = _fpcRepository.Single(x => x.Id == fpcId,
				"States",
				"States.OutConnectors",
				"States.InConnectors",
				"States.StateStations",
				"States.StateStations.Station",
				"States.StateStations.StateStationActivities",
				"States.StateStations.StateStationActivities.Activity",
				"States.StateStations.StateStationActivities.Activity.ActivityGroup",
				"States.StateStations.StateStationActivities.StateStationActivityMachines",
				"States.StateStations.StateStationActivities.StateStationActivityMachines.Machine",
				"Product",
				"Product.ProductGroup",
				"Product.ProductReworks");
			var clone = cloneModel(model);
			context.Commit();
			return clone;
		}
		protected FPC cloneModel(FPC model)
		{
			//clone FPC
			var clone = new FPC();
			clone.Name = model.Name;
			clone.Code = model.Code;
			clone.Product = model.Product;
			clone.IsDefault = model.IsDefault;
			clone.ModifiedBy = LoginInfo.Id;
			clone.CreatedDate = DateTime.Now;
			clone.ModifiedDate = DateTime.Now;
			clone.Status = (byte)Status.Active;

			//this array is used to match states for their connectors
			//a connector clone can find its start and end states by looking into this array
			var match = new List<KeyValuePair<State, State>>();
			
			//clone states
			foreach (var stateModel in model.States.ToArray())
			{
				var stateClone = stateDataService.Clone(stateModel);
				stateClone.FPC = model;
				clone.States.Add(stateClone);
				//add to match
				match.Add(new KeyValuePair<State, State>(stateModel, stateClone));
			}

			//clone connectors
			foreach (var startStateModel in model.States.ToArray())
			{
				//find the corresponding clone of startStateModel
				var cloneStartState = match.First(x => x.Key == startStateModel).Value;
				//find all out connectors of startStateModel
				foreach (var outConnectorModel in startStateModel.OutConnectors)
				{
					//find the corresponding clone of this connector's endState
					var cloneEndState = match.First(x => x.Key == outConnectorModel.EndState).Value;
					//add a connector clone to cloneStartState
					//no need to add the connector also to cloneEndState
					cloneStartState.OutConnectors.Add(new Connector
					{
						StartState = cloneStartState,
						EndState = cloneEndState,
					});
				}
			}

			//done
			if (FpcAdded != null)
				FpcAdded(this, new ModelAddedEventArgs<FPC>(clone));
			return clone;
		}

		/// <summary>
		/// Change the IsDefault value of the specified FPC to newValue
		/// <para>If set to true, undefaults the previously default fpcs</para>
		/// <para>If set to false, tries to make another fpc default</para>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="newValue"></param>
		internal void ChangeDefault(FPC model, bool newValue)
		{
			if (model.IsDefault == newValue)
				return;

			if (newValue)
			{
				// undefault the previously default fpcs
				var otherModels = _fpcRepository.Find(x => 
					x.IsDefault && 
					x.Product.Id == model.Product.Id && 
					x.Id != model.Id);
				foreach (var otherModel in otherModels)
				{
					otherModel.IsDefault = false;
				}

				//apply the new value to the specified fpc (parameter)
				model.IsDefault = true;
			}
			else
			{
				// try to make another fpc default
				var otherModels = _fpcRepository.Find(x =>
					x.Product.Id == model.Product.Id &&
					x.Id != model.Id);

				//if not the only fpc
				if (otherModels.Any())
				{
					//finds another default fpc
					bool found = false;
					foreach (var otherModel in otherModels)
					{
						if (!found && otherModel.IsDefault)
						{
							found = true;
						}
						else
						{
							otherModel.IsDefault = false;
						}
					}

					//if no other default fpc, make the first one default
					if (!found)
					{
						otherModels.First().IsDefault = true;
					}

					//apply the new value to the specified fpc (parameter)
					model.IsDefault = false;
				}
				else
					//if no other fpc available, throw
					throw new Exception("این تنها FPC برای این محصول است.\nلذا به ناچار همین FPC پیش فرض است");
			}

			context.SaveChanges();
		}

		public IEnumerable<ProductRework> GetProductReworks(FPC model, bool includeMainProduct)
		{
			var list = new List<ProductRework>();
			var cRepos = new Repository<ProductRework>(context);
			var models = cRepos.Find(x =>
				x.Product.Id == model.Product.Id
				&& (includeMainProduct || x.Rework != null),
				"Product", "Rework");
			foreach (var item in models)
			{
				list.Add(item);
			}
			return list;
		}
		internal List<State> GetDestinationStates(int startStateId)
		{
			var startState = stateDataService.GetSingle(startStateId);
			return startState.OutConnectors.Where(conn =>
				conn.EndState.StateType == Common.StateType.Mid ||
				conn.EndState.StateType == Common.StateType.Final)
				.Select(conn => conn.EndState).ToList();
		}

		internal void CorrectFPCStates(Model.FPC model)
		{
			//stateDataService.AddModel does not need commit but it has

			//add start state
			if (!model.States.Any(x => x.StateTypeNr == (int)StateType.Start))
				stateDataService.AddModel(new State
				{
					FPC = model,
					X = 400,
					Y = 100,
					Name = "شروع",
					Code = "",
					StateType = StateType.Start
				});
			//add final state
			if (!model.States.Any(x => x.StateType == StateType.Final))
				stateDataService.AddModel(new State
				{
					FPC = model,
					X = 50,
					Y = 100,
					Name = "محصول نهایی",
					Code = "",
					StateType = StateType.Final
				});

			//add rework states for the newly added productReworks
			if (!model.Product.ProductReworks.Any(x=>x.Rework == null))
				model.Product.ProductReworks.Add(
					new ProductRework
					{
						Product = model.Product,
						Code = model.Product.Code + "[Main]",
						Name = model.Product.Name,
						Rework = null,
						ModifiedBy = LoginInfo.Id,
					});

			int reworkStateCounter = 0;
			foreach (var productRework in model.Product.ProductReworks.Where(x => x.Rework != null))
			{
				if (!model.States.Any(x =>
					x.StateTypeNr == (int)StateType.Rework
					&& x.OnProductRework != null
					&& x.OnProductRework.Id == productRework.Id))
				{
					stateDataService.AddModel(new State
					{
						FPC = model,
						X = 20,
						Y = reworkStateCounter * 35 + 500,
						Name = productRework.Name,
						Code = productRework.Code,
						OnProductRework = productRework,
						StateType = StateType.Rework,
					});
				}
				reworkStateCounter++;
			}
			//set pr of a state to mainPR if it's null
			var states = model.States.Where(x =>
				x.StateTypeNr == (int)StateType.Mid
				&& x.OnProductRework == null);
			foreach (var state in states)
			{
				state.OnProductRework = model.Product.ProductReworks.First(x => x.Rework == null);
			}
			//...
			context.SaveChanges();
		}
	}
}