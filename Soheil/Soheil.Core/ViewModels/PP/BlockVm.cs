﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Soheil.Common;
using Soheil.Common.SoheilException;

namespace Soheil.Core.ViewModels.PP
{
	/// <summary>
	/// A Block is a sequence of Tasks with the same Station and State with no space in between.
	/// <para>Tasks of a Block can contain different number of Operators, thus can have different CycleTimes</para>
	/// <para>Blocks can be added or removed directly from PPTable</para>
	/// </summary>
	public class BlockVm : PPItemVm
	{
		PPTableVm _ppTable;
		Model.Block _model;
		/// <summary>
		/// Gets Id property of the model representing this ViewModel
		/// </summary>
		public override int Id { get { return _model.Id; } }
		/// <summary>
		/// Gets DataService for Block
		/// </summary>
		public DataServices.BlockDataService BlockDataService { get { return _ppTable.BlockDataService; } }

        //Parent Dependency Property
        public Core.PP.PPItemCollection Parent
        {
            get { return (Core.PP.PPItemCollection)GetValue(ParentProperty); }
            set { SetValue(ParentProperty, value); }
        }
        public static readonly DependencyProperty ParentProperty =
            DependencyProperty.Register("Parent", typeof(Core.PP.PPItemCollection), typeof(BlockVm), new UIPropertyMetadata(null));

		#region Ctor, reload
        public BlockVm(Model.Block model, Core.PP.PPItemCollection parent, int stationIndex)
		{
			_model = model;
            _ppTable = parent.PPTable;
            Parent = parent;
			RowIndex = stationIndex;
			StartDateTime = model.StartDateTime;
			DurationSeconds = model.DurationSeconds;
			initializeCommands();
		}

		/// <summary>
		/// Reloads current blocks info from database (according to its id)
		/// </summary>
		public void Reload()
		{
			var data = new Soheil.Core.PP.BlockFullData();
			data.Model = BlockDataService.GetSingleFull(Id);
			data.ReportData = BlockDataService.GetProductionReportData(data.Model);
			data.CanAddSetupBefore = BlockDataService.CanAddSetupBeforeBlock(data.Model);
			_model = data.Model;
			Reload(data);
		}
		/// <summary>
		/// Reloads current blocks info from the given parameter
		/// </summary>
		/// <param name="data"></param>
		public void Reload(Soheil.Core.PP.BlockFullData data)
		{
			//fill the vm from data.Model
			//Product and State
			ProductId = data.Model.StateStation.State.FPC.Product.Id;
			ProductCode = data.Model.StateStation.State.FPC.Product.Code;
			ProductName = data.Model.StateStation.State.FPC.Product.Name;
			ProductColor = data.Model.StateStation.State.FPC.Product.Color;
			StateCode = data.Model.StateStation.State.Code;
			IsRework = data.Model.StateStation.State.IsReworkState == Bool3.True;
			//Block background texts
			BlockTargetPoint = data.Model.BlockTargetPoint;
			BlockProducedG1 = data.ReportData[0];
			ReportFillPercent = string.Format("{0:D2}%", data.ReportData[1]);
			IsReportFilled = (data.ReportData[1] >= 100);
			//Navigation
			//job
			if (data.Model.Job != null)
			{
				Job = new PPJobVm(data.Model.Job);
				if (Parent.PPTable.SelectedJobId == Job.Id)
					IsJobSelected = true;
			}
			//tasks
			foreach (var task in data.Model.Tasks)
			{
				TaskList.Add(new PPTaskVm(task, this));
			}
			if (Parent.PPTable.SelectedBlock == null) ViewMode = PPViewMode.Simple;
			else ViewMode = (Parent.PPTable.SelectedBlock.Id == Id) ? PPViewMode.Report : PPViewMode.Simple;
		}
		#endregion

		#region Product and State
		/// <summary>
		/// Product Id
		/// </summary>
		public int ProductId { get; set; }
		//ProductCode Dependency Property
		public string ProductCode
		{
			get { return (string)GetValue(ProductCodeProperty); }
			set { SetValue(ProductCodeProperty, value); }
		}
		public static readonly DependencyProperty ProductCodeProperty =
			DependencyProperty.Register("ProductCode", typeof(string), typeof(BlockVm), new UIPropertyMetadata(null));
		//ProductName Dependency Property
		public string ProductName
		{
			get { return (string)GetValue(ProductNameProperty); }
			set { SetValue(ProductNameProperty, value); }
		}
		public static readonly DependencyProperty ProductNameProperty =
			DependencyProperty.Register("ProductName", typeof(string), typeof(BlockVm), new UIPropertyMetadata(null));
		//ProductColor Dependency Property
		public Color ProductColor
		{
			get { return (Color)GetValue(ProductColorProperty); }
			set { SetValue(ProductColorProperty, value); }
		}
		public static readonly DependencyProperty ProductColorProperty =
			DependencyProperty.Register("ProductColor", typeof(Color), typeof(BlockVm), new UIPropertyMetadata(Colors.White, (d, e) =>
			d.SetValue(ForeColorProperty, new SolidColorBrush(((Color)e.NewValue).IsDark() ? Colors.White : Colors.Black))));
		public static readonly DependencyProperty ForeColorProperty =
			DependencyProperty.Register("ForeColor", typeof(SolidColorBrush), typeof(BlockVm), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));
		//StateCode Dependency Property
		public string StateCode
		{
			get { return (string)GetValue(StateCodeProperty); }
			set { SetValue(StateCodeProperty, value); }
		}
		public static readonly DependencyProperty StateCodeProperty =
			DependencyProperty.Register("StateCode", typeof(string), typeof(BlockVm), new UIPropertyMetadata(null));
		//IsRework Dependency Property
		public bool IsRework
		{
			get { return (bool)GetValue(IsReworkProperty); }
			set { SetValue(IsReworkProperty, value); }
		}
		public static readonly DependencyProperty IsReworkProperty =
			DependencyProperty.Register("IsRework", typeof(bool), typeof(BlockVm), new UIPropertyMetadata(false)); 
		#endregion

		#region Block background texts
		//BlockTargetPoint Dependency Property
		public int BlockTargetPoint
		{
			get { return (int)GetValue(BlockTargetPointProperty); }
			set { SetValue(BlockTargetPointProperty, value); }
		}
		public static readonly DependencyProperty BlockTargetPointProperty =
			DependencyProperty.Register("BlockTargetPoint", typeof(int), typeof(BlockVm), new UIPropertyMetadata(0));
		//BlockProducedG1 Dependency Property
		public int BlockProducedG1
		{
			get { return (int)GetValue(BlockProducedG1Property); }
			set { SetValue(BlockProducedG1Property, value); }
		}
		public static readonly DependencyProperty BlockProducedG1Property =
			DependencyProperty.Register("BlockProducedG1", typeof(int), typeof(BlockVm), new UIPropertyMetadata(0));
		//ReportFillPercent Dependency Property
		public string ReportFillPercent
		{
			get { return (string)GetValue(ReportFillPercentProperty); }
			set { SetValue(ReportFillPercentProperty, value); }
		}
		public static readonly DependencyProperty ReportFillPercentProperty =
			DependencyProperty.Register("ReportFillPercent", typeof(string), typeof(BlockVm), new UIPropertyMetadata("0"));
		//IsReportFilled Dependency Property
		public bool IsReportFilled
		{
			get { return (bool)GetValue(IsReportFilledProperty); }
			set { SetValue(IsReportFilledProperty, value); }
		}
		public static readonly DependencyProperty IsReportFilledProperty =
			DependencyProperty.Register("IsReportFilled", typeof(bool), typeof(BlockVm), new UIPropertyMetadata(false));
		#endregion

		#region Tasks,Job,Report
		public ObservableCollection<PPTaskVm> TaskList { get { return _taskList; } }
		private ObservableCollection<PPTaskVm> _taskList = new ObservableCollection<PPTaskVm>();

		//Job Dependency Property
		public PPJobVm Job
		{
			get { return (PPJobVm)GetValue(JobProperty); }
			set { SetValue(JobProperty, value); }
		}
		public static readonly DependencyProperty JobProperty =
			DependencyProperty.Register("Job", typeof(PPJobVm), typeof(BlockVm), new UIPropertyMetadata(null));

		//BlockReport Dependency Property
		public BlockReportVm BlockReport
		{
			get { return (BlockReportVm)GetValue(BlockReportProperty); }
			set { SetValue(BlockReportProperty, value); }
		}
		public static readonly DependencyProperty BlockReportProperty =
			DependencyProperty.Register("BlockReport", typeof(BlockReportVm), typeof(BlockVm), new UIPropertyMetadata(null)); 
		#endregion

		//CanAddSetupBefore Dependency Property
		public bool CanAddSetupBefore
		{
			get { return (bool)GetValue(CanAddSetupBeforeProperty); }
			set { SetValue(CanAddSetupBeforeProperty, value); }
		}
		public static readonly DependencyProperty CanAddSetupBeforeProperty =
			DependencyProperty.Register("CanAddSetupBefore", typeof(bool), typeof(BlockVm), new UIPropertyMetadata(true));
		//IsEditMode Dependency Property
		public bool IsEditMode
		{
			get { return (bool)GetValue(IsEditModeProperty); }
			set { SetValue(IsEditModeProperty, value); }
		}
		public static readonly DependencyProperty IsEditModeProperty =
			DependencyProperty.Register("IsEditMode", typeof(bool), typeof(BlockVm), new UIPropertyMetadata(false));
		//IsJobSelected Dependency Property
		/// <summary>
		/// To use this you must set SelectedJobId on PPTable to this.Job.Id then reload this block
		/// </summary>
		public bool IsJobSelected
		{
			get { return (bool)GetValue(IsJobSelectedProperty); }
			set { SetValue(IsJobSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsJobSelectedProperty =
			DependencyProperty.Register("IsJobSelected", typeof(bool), typeof(BlockVm),
			new UIPropertyMetadata(false));


		#region Commands

		void initializeCommands()
		{
			ReloadBlockCommand = new Commands.Command(o => Reload());
            AddBlockToEditorCommand = new Commands.Command(o =>
			{
				try
				{
                    if (_model == null) return;
                    _ppTable.TaskEditor.BlockList.Add(new Soheil.Core.ViewModels.PP.Editor.PPEditorBlock(_model));
                    _ppTable.TaskEditor.SelectedBlock = _ppTable.TaskEditor.BlockList.Last();
				}
				catch (Exception exp) { Message.AddEmbeddedException(exp.Message); }
			});
			EditItemCommand = new Commands.Command(o =>
			{
				try
				{
					_ppTable.TaskEditor.Reset();
					_ppTable.TaskEditor.IsVisible = true;
					_ppTable.JobEditor.IsVisible = false;
                    _ppTable.TaskEditor.BlockList.Add(new Soheil.Core.ViewModels.PP.Editor.PPEditorBlock(_model));
                    _ppTable.TaskEditor.SelectedBlock = _ppTable.TaskEditor.BlockList.Last();
				}
				catch (Exception exp) { Message.AddEmbeddedException(exp.Message); }
			});
			AddJobToEditorCommand = new Commands.Command(o =>
			{
				try
				{
					_ppTable.JobEditor.Append(Job);
				}
				catch (Exception exp) { Message.AddEmbeddedException(exp.Message); }
			}, () =>
			{
				if (Job == null) return false;
				if (Job.Id == 0) return false;
				return true;
			});
			EditJobCommand = new Commands.Command(o =>
			{
				try
				{
					_ppTable.TaskEditor.IsVisible = false;
					_ppTable.JobEditor.IsVisible = true;
					_ppTable.JobEditor.Reset();
					_ppTable.JobEditor.Append(Job);
				}
				catch (Exception exp) { Message.AddEmbeddedException(exp.Message); }
			}, () =>
			{
				if (Job == null) return false;
				if (Job.Id == 0) return false;
				return true;
			});
			EditReportCommand = new Commands.Command(o =>
			{
				//try
				{
					BlockReport = new BlockReportVm(this);
					BlockReport.ReloadProcessReportRows();
                    _ppTable.SelectedBlock = this;
                }
				//catch (Exception exp)
				{
				//	Message.AddEmbeddedException(exp.Message);
				}
			});
			DeleteItemCommand = new Commands.Command(o =>
			{
				try
				{
					_ppTable.BlockDataService.DeleteModelById(Id);
					Parent.RemoveItem(this);
				}
				catch (Exception exp)
				{
					Message.AddEmbeddedException(exp.Message);
				}
			});
			DeleteJobCommand = new Commands.Command(o =>
			{
				try
				{
					_ppTable.JobDataService.DeleteModel(Job.Id);
					_ppTable.RemoveBlocks(Job);
				}
				catch (RoutedException exp)
				{
					if(exp.Target is PPTaskVm)
						(exp.Target as PPTaskVm).Message.AddEmbeddedException(exp.Message);
					else //if(exp.Target is BlockVm)
						Message.AddEmbeddedException(exp.Message);
				}
				catch (Exception exp)
				{
					Message.AddEmbeddedException(exp.Message);
				}
			}, () => { return Job != null; });
			InsertSetupBefore = new Commands.Command(async o =>
			{
				var tmp = BlockDataService;
				var result = await Task.Run(() => tmp.InsertSetupBeforeBlock(Id));
				//var result = tmp.InsertSetupBeforeTask(Id);
				if (result.IsSaved)
				{
					Parent.Reload();
				}
				else
				{
					if (result.IsSaved) return;
					Message.AddEmbeddedException("قادر به افزودن آماده سازی نمی باشد.\nبرخی از Taskهای بعدی در این ایستگاه قابل تغییر نیستند.");
					foreach (var error in result.Errors)
					{
						switch (error.Value1)
						{
							case Soheil.Core.DataServices.BlockDataService.InsertSetupBeforeBlockErrors.ErrorSource.Task:
								var task = TaskList.FirstOrDefault(x => x.Id == error.Value3);
								if (task != null) task.Message.AddEmbeddedException(error.Value2);
								else Message.AddEmbeddedException(error.Value2);
								break;
							case Soheil.Core.DataServices.BlockDataService.InsertSetupBeforeBlockErrors.ErrorSource.NPT:
								var npt = Parent[this.RowIndex].NPTs.FirstOrDefault(x => x.Id == error.Value3);
								if (npt != null) npt.Message.AddEmbeddedException(error.Value2);
								else Message.AddEmbeddedException(error.Value2);
								break;
							case Soheil.Core.DataServices.BlockDataService.InsertSetupBeforeBlockErrors.ErrorSource.This:
								Message.AddEmbeddedException(error.Value2);
								break;
							default:
								break;
						}
					}
				}
			});
		}
		//ReloadBlockCommand Dependency Property
		public Commands.Command ReloadBlockCommand
		{
			get { return (Commands.Command)GetValue(ReloadBlockCommandProperty); }
			set { SetValue(ReloadBlockCommandProperty, value); }
		}
		public static readonly DependencyProperty ReloadBlockCommandProperty =
			DependencyProperty.Register("ReloadBlockCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
        //AddBlockToEditorCommand Dependency Property
        public Commands.Command AddBlockToEditorCommand
		{
            get { return (Commands.Command)GetValue(AddBlockToEditorCommandProperty); }
            set { SetValue(AddBlockToEditorCommandProperty, value); }
		}
        public static readonly DependencyProperty AddBlockToEditorCommandProperty =
            DependencyProperty.Register("AddBlockToEditorCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		//AddJobToEditorCommand Dependency Property
		public Commands.Command AddJobToEditorCommand
		{
			get { return (Commands.Command)GetValue(AddJobToEditorCommandProperty); }
			set { SetValue(AddJobToEditorCommandProperty, value); }
		}
		public static readonly DependencyProperty AddJobToEditorCommandProperty =
			DependencyProperty.Register("AddJobToEditorCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		//EditJobCommand Dependency Property
		public Commands.Command EditJobCommand
		{
			get { return (Commands.Command)GetValue(EditJobCommandProperty); }
			set { SetValue(EditJobCommandProperty, value); }
		}
		public static readonly DependencyProperty EditJobCommandProperty =
			DependencyProperty.Register("EditJobCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		//DeleteJobCommand Dependency Property
		public Commands.Command DeleteJobCommand
		{
			get { return (Commands.Command)GetValue(DeleteJobCommandProperty); }
			set { SetValue(DeleteJobCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteJobCommandProperty =
			DependencyProperty.Register("DeleteJobCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		//InsertSetupBefore Dependency Property
		public Commands.Command InsertSetupBefore
		{
			get { return (Commands.Command)GetValue(InsertSetupBeforeProperty); }
			set { SetValue(InsertSetupBeforeProperty, value); }
		}
		public static readonly DependencyProperty InsertSetupBeforeProperty =
			DependencyProperty.Register("InsertSetupBefore", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		#endregion
	}
}
