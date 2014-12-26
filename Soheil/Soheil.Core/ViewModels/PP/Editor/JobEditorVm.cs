using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using S = System;
namespace Soheil.Core.ViewModels.PP.Editor
{
	/// <summary>
	/// This is the whole Editor
	/// </summary>
	public class JobEditorVm : DependencyObject
	{
		public event Action RefreshPPItems;

		DataServices.ProductGroupDataService _productGroupDs;
		DataServices.FPCDataService _fpcDs;
		DataServices.JobDataService _jobDs;

		Dal.SoheilEdmContext _uow;
		Core.ViewModels.PP.PricingAI.PricingVm _pricings;

		public JobEditorVm(Soheil.Core.ViewModels.PP.PricingAI.PricingVm jobsSource = null)
		{
			_pricings = jobsSource;

			initializeDataServices();
			initializeCommands();

			//load products
			var pgList = _productGroupDs.GetActives();
			foreach (var pg in pgList)
			{
				var pgvm = new JobProductGroupVm(pg, _jobDs);
				AllProductGroups.Add(pgvm);
			}

			//event handler for DeleteJobCommand
			JobList.CollectionChanged += (s, e) =>
			{
				if(e.NewItems != null)
					foreach (var item in e.NewItems.OfType<PPEditorJob>())
					{
						item.JobDeleted += job => JobList.Remove(job);
						item.RefreshPPTable += () => { if (RefreshPPItems != null) RefreshPPItems(); };
					}
			};

			//add jobs source
			if(jobsSource!=null)
			{
				foreach (var result in jobsSource.Results.Where(x=>x.Selected))
				{
					bool done = false;
					
					//find product
					foreach (var pg in pgList)
					{
						var p = pg.Products.FirstOrDefault(x => x.Id == result.Id);
						if(p!=null)
						{
							//Create Jobs
							for (int i = 0; i < jobsSource.Periods.Count; i++)
							{
								//job model
								var model = new Model.Job
								{
									Code = "Auto",
									ReleaseTime = jobsSource.Periods[i].StartDate,
									Deadline = jobsSource.Periods[i].EndDate,
									Description = "Created Using Pricing AI",
									FPC = p.DefaultFpc,
									ProductRework = p.MainProductRework,
									Quantity = result.Production[i],
									Weight = 1f,
								};
								//Add to vm
								var job = new JobVm(model);
								Append(job);
							}
							done = true;
							break;
						}
						if (done) break;
					}

					//if not found
					if(!done)
					{
						//???
					}
				}
			}
		}

		void initializeDataServices()
		{
			_uow = new Dal.SoheilEdmContext();
			_productGroupDs = new DataServices.ProductGroupDataService(_uow);
			_fpcDs = new DataServices.FPCDataService(_uow);
			_jobDs = new DataServices.JobDataService(_uow);
		}

		#region Interactions
		//Add
		/*void FpcViewer_AddNewJob(Fpc.StateVm fpcState)
		{
			var ppStateListItem = PPStateList.FirstOrDefault(x => x.StateId == fpcState.Id);
			if (ppStateListItem == null)
			{
				ppStateListItem = new PPEditorState(fpcState);
				PPStateList.Add(ppStateListItem);
			}
		}
		//Remove
		public void FpcViewer_RemovePPState(PPEditorState ppState)
		{
			if (SelectedState != null && SelectedState.StateId == ppState.StateId) SelectedState = null;
			PPStateList.Remove(ppState);
		}*/
		//Clear
		public void Reset()
		{
			JobList.Clear();
		}
		#endregion

		#region Product etc
		//All Job ProductGroups Observable Collection
		private ObservableCollection<JobProductGroupVm> _allProductGroups = new ObservableCollection<JobProductGroupVm>();
		public ObservableCollection<JobProductGroupVm> AllProductGroups { get { return _allProductGroups; } }
		//JobList Observable Collection
		private ObservableCollection<PPEditorJob> _jobList = new ObservableCollection<PPEditorJob>();
		public ObservableCollection<PPEditorJob> JobList { get { return _jobList; } }
		#endregion

		#region AI
		/// <summary>
		/// Gets or sets a bindable value that indicates FreeSpaceLength
		/// </summary>
		public float FreeSpaceLength
		{
			get { return (float)GetValue(FreeSpaceLengthProperty); }
			set { SetValue(FreeSpaceLengthProperty, value); }
		}
		public static readonly DependencyProperty FreeSpaceLengthProperty =
			DependencyProperty.Register("FreeSpaceLength", typeof(float), typeof(JobEditorVm),
			new PropertyMetadata(1f, (d, e) => { }, (d, v) => { return ((float)v < 1f) ? 1f : v; }));

		void createDataSet()
		{
			if (!JobList.Any()) return;

			DateTime start = JobList.Min(x => x.ReleaseDT);
			DateTime end = JobList.Max(x => x.Deadline);
			if (_pricings != null)
			{
				DateTime periodsStart = _pricings.Periods.Min(x => x.StartDate);
				DateTime periodsEnd = _pricings.Periods.Max(x => x.EndDate);
				if (periodsStart < start) start = periodsStart;
				if (periodsEnd > end) end = periodsEnd;
			}
			int duration = (int)((end - start).TotalSeconds * FreeSpaceLength);

			//create root (stations, tasks and products)
			var taskDs = new DataServices.TaskDataService();
			var root = taskDs.GetItemsAndSpaces(start, duration);

			//fill jobs
			root.Jobs = new List<Core.PP.PlannerAI.Job>();
			int jobIndex = 0;
			int jobGroupId = 0;
			foreach (var job in JobList)
			{
				//add job
				var j = new Core.PP.PlannerAI.Job();
				j.Quantity = job.Quantity;
				j.ReleaseTime = (int)(job.ReleaseDT - start).TotalSeconds;
				j.Deadline = (int)(job.Deadline - start).TotalSeconds;
				j.DelayCount = job.LagCount;
				j.DelayTime = job.LagSeconds;
				j.Weight = job.Weight;
				j.GroupId = jobGroupId++;
				for (int i = 0; i < job.Replications.Count; i++)
				{
					j.Index = jobIndex++;
					root.Jobs.Add(j);
				}

				//set job's product and add if not exist
				var p = root.Products.FirstOrDefault(x => x.Id == job.Product.Id);
				if (p == null)
				{
					p = new Core.PP.PlannerAI.Product(job.Product.Model);
					root.Products.Add(p);
				}
				j.Product = p;
			}

			//station index helper
			int[] stationIndices = new int[root.Stations.Max(x=>x.Id)+1];
			for (int i = 0; i < root.Stations.Count; i++)
			{
				stationIndices[root.Stations[i].Id] = i;
			}

			//changeovers
			root.Changeovers = new int[root.Stations.Count, root.Products.Count, root.Products.Count];
			var chds = new DataServices.ChangeoverDataService();
			var wuds = new DataServices.WarmupDataService();
			for (int s = 0; s < root.Stations.Count; s++)
			{
				for (int i = 0; i < root.Products.Count; i++)
				{
					//find warmup
					var wu = wuds.SmartFind(root.Products[i].MainProductReworkId, root.Stations[s].Id);
					//set warmup
					if (wu != null)
						root.Changeovers[s, i, i] = wu.Seconds;
					for (int j = 0; j < root.Products.Count; j++)
					{
						//skip warmup
						if (i == j) continue;
						//find changeover
						var ch = chds.SmartFind(
							root.Products[i].MainProductReworkId,
							root.Products[j].MainProductReworkId,
							root.Stations[s].Id);
						//set changeover + warmup
						if (ch != null) 
							root.Changeovers[s, i, j] = ch.Seconds + root.Changeovers[s, i, i];
					}
				}
			}

			//create fpc and complete products
			for (int p = 0; p < root.Products.Count; p++)
			{
				root.Products[p].Index = p;
				var fpc = _fpcDs.GetActiveForProduct(root.Products[p].Id);

				//trace fpc
				var dic = new Dictionary<Model.State, int>();
				var finalState = fpc.States.FirstOrDefault(x => x.StateType == Common.StateType.Final);
				if(!traceFpc(finalState, dic))
				{
					MessageBox.Show(string.Format("Loop in FPC of product {0}", fpc.Product.Name));
					return;
				}
				else if (!dic.Any())
				{
					MessageBox.Show(string.Format("FPC of product {0} is empty or invalid", fpc.Product.Name));
					return;
				}

				int maxLevel = dic.Max(x => x.Value);
				root.Products[p].Levels = new Core.PP.PlannerAI.Level[maxLevel+1];
				for (int l = 0; l <= maxLevel; l++)//higher levels are later
				{
					//create level
					var level = new Soheil.Core.PP.PlannerAI.Level();
					root.Products[p].Levels[l] = level;
					level.Index = l;

					//find models in this level
					var states = dic.Where(x => maxLevel - x.Value == l).Select(x => x.Key);

					//create states
					level.States = states.Select(s => new Core.PP.PlannerAI.State
					{
						Id = s.Id,
						Index = 0,
						//create ss
						SSList = s.StateStations.Select(ss => new Core.PP.PlannerAI.StateStation
						{
							Id = ss.Id,
							StationIndex = stationIndices[ss.Station.Id],
							CycleTime = ss.MaxCycleTime,
						}).ToList(),
						//create requisites
					}).ToList();
					level.States.ForEach(s =>
					{
						s.Requisites = states.FirstOrDefault(x => x.Id == s.Id)
							.Requisites.Select(x =>
								root.Products[p].FindLevelState(s.Id))
								.ToList();
					});
				}
			}

			var w = new System.IO.StreamWriter("jdata.txt");
			w.Write(root.ToString());
			w.Close();
		}



		bool traceFpc(Model.State last, Dictionary<Model.State, int> dic)
		{
			//add last level of states
			addRequsities(last, dic, 0);
			for (int level = 1; level < 100; level++)//higher levels are earlier
			{
				//find requisites in its previous level
				var requisites = dic.Where(x => x.Value == level).Select(x => x.Key);
				if (!requisites.Any()) return true;
				//add these requisites
				foreach (var state in requisites)
				{
					addRequsities(state, dic, level);
				}
			}
			return false;//loop
		}
		void addRequsities(Model.State last, Dictionary<Model.State, int> dic, int level)
		{
			//add all nodes from previous level to dic
			foreach (var state in last.Requisites)
			{
				int val;
				if (dic.TryGetValue(state, out val))
				{
					//correct node level
					if (val < level)
					{
						dic.Remove(state);
						dic.Add(state, level);
					}
				}
				else
				{
					//add node
					dic.Add(state, level);
				}
			}
		}
		#endregion

		//IsVisible Dependency Property
		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
		public static readonly DependencyProperty IsVisibleProperty =
			DependencyProperty.Register("IsVisible", typeof(bool), typeof(JobEditorVm), new UIPropertyMetadata(false));

		#region Commands
		void initializeCommands()
		{
			AutoPlanCommand = new Commands.Command(o =>
			{
				createDataSet();
			});
			SaveAllCommand = new Commands.Command(o =>
			{
				foreach (var job in JobList.Where(x => x.Quantity > 0))
				{
					job.SaveCommand.Execute(false);//false: job won't refresh pptable
				}
				Reset();
				IsVisible = false;
				if (RefreshPPItems != null)
					RefreshPPItems();
			});
			ClearAllCommand = new Commands.Command(o => Reset());
			ExitCommand = new Commands.Command(o => IsVisible = false);
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates AutoPlanCommand
		/// </summary>
		public Commands.Command AutoPlanCommand
		{
			get { return (Commands.Command)GetValue(AutoPlanCommandProperty); }
			set { SetValue(AutoPlanCommandProperty, value); }
		}
		public static readonly DependencyProperty AutoPlanCommandProperty =
			DependencyProperty.Register("AutoPlanCommand", typeof(Commands.Command), typeof(JobEditorVm), new PropertyMetadata(null));

		//SaveAllCommand Dependency Property
		public Commands.Command SaveAllCommand
		{
			get { return (Commands.Command)GetValue(SaveAllCommandProperty); }
			set { SetValue(SaveAllCommandProperty, value); }
		}
		public static readonly DependencyProperty SaveAllCommandProperty =
			DependencyProperty.Register("SaveAllCommand", typeof(Commands.Command), typeof(JobEditorVm), new UIPropertyMetadata(null));
		//ClearAllCommand Dependency Property
		public Commands.Command ClearAllCommand
		{
			get { return (Commands.Command)GetValue(ClearAllCommandProperty); }
			set { SetValue(ClearAllCommandProperty, value); }
		}
		public static readonly DependencyProperty ClearAllCommandProperty =
			DependencyProperty.Register("ClearAllCommand", typeof(Commands.Command), typeof(JobEditorVm), new UIPropertyMetadata(null));
		//ExitCommand Dependency Property
		public Commands.Command ExitCommand
		{
			get { return (Commands.Command)GetValue(ExitCommandProperty); }
			set { SetValue(ExitCommandProperty, value); }
		}
		public static readonly DependencyProperty ExitCommandProperty =
			DependencyProperty.Register("ExitCommand", typeof(Commands.Command), typeof(JobEditorVm), new UIPropertyMetadata(null)); 
		#endregion

		internal void Append(JobVm Job)
		{
			var job = new Editor.PPEditorJob(Job.Model, _jobDs);
			job.RefreshPPTable += () => { if (RefreshPPItems != null) RefreshPPItems(); };
            JobList.Add(job);
		}
	}
}
