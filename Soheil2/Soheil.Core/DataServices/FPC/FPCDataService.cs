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

		Repository<FPC> fpcRepository;

		StateDataService stateDataService;
		ConnectorDataService connectorDataService;

		public FPCDataService()
			: this(new SoheilEdmContext())
		{
		}
		public FPCDataService(SoheilEdmContext context)
		{
			this.context = context;
			fpcRepository = new Repository<FPC>(context);

			//other dataservices
			stateDataService = new StateDataService(context);
			connectorDataService = new ConnectorDataService(context);
		}

		public IEnumerable<FPC> GetAllForProduct(int productId)
		{
			var list = new List<FPC>();
			var models = fpcRepository.Find(x => x.Product.Id == productId, "Product");
			foreach (var item in models)
			{
				list.Add(item);
			}
			return list;
		}

		public FPC GetActiveForProduct(int productId)
		{
			var model = fpcRepository.FirstOrDefault(x => x.Product.Id == productId && x.IsDefault, "Product");
			return model;
		}
		/// <summary>
		/// Save all changes to this context
		/// </summary>
		public void ApplyChanges()
		{
			context.Commit();
		}

		//IDataService
		public FPC GetSingle(int id)
		{
			return fpcRepository.FirstOrDefault(x => x.Id == id, "Product");
		}
		public FPC GetSingleWithStates(int id)
		{
			return fpcRepository.FirstOrDefault(x => x.Id == id, "Product", "States", "States.OnProductRework");
		}

		public ObservableCollection<FPC> GetAll()
		{
			var models = fpcRepository.GetAll("Product");
			return new ObservableCollection<FPC>(models);
		}

		public ObservableCollection<FPC> GetActives()
		{
			var models = fpcRepository.Find(x => x.IsDefault, "Product");
			return new ObservableCollection<FPC>(models);
		}

		public int AddModel(FPC model)
		{
			throw new NotImplementedException();
		}

		public int AddModel(FPC model, int groupId)
		{
			model.Product = new Repository<Product>(context).Single(group => group.Id == groupId);
			model.CreatedDate = DateTime.Now;
			//product.FPCs.Add(model);
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


		/// <summary>
		/// creates and saves a clone of the given model (id) and returns it
		/// </summary>
		/// <param name="id">Id of source model</param>
		/// <returns>clone</returns>
		internal FPC CloneModelById(int fpcId)
		{
			var model = fpcRepository.Single(x => x.Id == fpcId,
				"WorkDays",
				"WorkShiftPrototypes",
				"WorkDays.WorkShifts",
				"WorkDays.WorkShifts.WorkShiftPrototype",
				"WorkDays.WorkShifts.WorkBreaks");
			var clone = cloneModel(model);
			context.SaveChanges();
			return clone;
		}
		protected FPC cloneModel(FPC model)
		{
			var clone = new FPC();
			clone.Name = model.Name;
			clone.Code = model.Code;
			clone.Product = model.Product;
			clone.IsDefault = model.IsDefault;
			clone.ModifiedBy = LoginInfo.Id;
			clone.CreatedDate = DateTime.Now;
			clone.ModifiedDate = DateTime.Now;
			clone.Status = (byte)Status.Active;
			foreach (var stateModel in model.States.ToArray())
			{
				var stateClone = stateDataService.Clone(stateModel);
				stateClone.FPC = model;
				clone.States.Add(stateClone);
			}
			if (FpcAdded != null)
				FpcAdded(this, new ModelAddedEventArgs<FPC>(clone));
			return clone;
		}

		internal void ChangeDefault(FPC model, bool newValue)
		{
			if (model.IsDefault == newValue)
				return;

			var otherModels = fpcRepository.Find(x => x.Product.Id == model.Product.Id && x.Id != model.Id);
			if (newValue)
			{
				otherModels.Select(x => x.IsDefault = false);
				model.IsDefault = true;
			}
			else
			{
				if (otherModels.Any())
				{
					otherModels.First().IsDefault = true;
					model.IsDefault = false;
				}
				else
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
			//add start state
			if (!model.States.Any(x => x.StateTypeNr == (int)StateType.Start))
				stateDataService.AddModel(new State
				{
					FPC = model,
					X = 50,
					Y = 50,
					Name = "شروع",
					Code = "",
					StateType = StateType.Start
				});
			//add final state
			if (!model.States.Any(x => x.StateType == StateType.Final))
				stateDataService.AddModel(new State
				{
					FPC = model,
					X = 300,
					Y = 50,
					Name = "محصول نهایی",
					Code = "",
					StateType = StateType.Final
				});

			//add rework states for the newly added productReworks
			var prs = model.Product.ProductReworks;
			int reworkStateCounter = 0;
			var rnd = new Random();
			foreach (var productRework in prs.Where(x => x.Rework != null))
			{
				if (!model.States.Any(x =>
					x.StateTypeNr == (int)StateType.Rework
					&& x.OnProductRework != null
					&& x.OnProductRework.Id == productRework.Id))
				{
					stateDataService.AddModel(new State
					{
						FPC = model,
						X = (++reworkStateCounter) * 50 + rnd.Next(-20, 20),
						Y = reworkStateCounter * 50 + 200 + rnd.Next(-20, 20),
						Name = productRework.Name,
						Code = productRework.Code,
						OnProductRework = productRework,
						StateType = StateType.Rework,
					});
				}
			}
			//set pr of a state to mainPR if it's null
			var states = model.States.Where(x =>
				x.StateTypeNr == (int)StateType.Mid
				&& x.OnProductRework == null);
			foreach (var state in states)
			{
				state.OnProductRework = prs.First(x => x.Rework == null);
			}
			//...
			context.SaveChanges();
		}
	}
}