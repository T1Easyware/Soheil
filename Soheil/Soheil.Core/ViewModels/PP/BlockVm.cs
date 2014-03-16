using System;
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
		Model.Block _model;
		/// <summary>
		/// Gets Id property of the model representing this ViewModel
		/// </summary>
		public override int Id { get { return _model.Id; } }
		/// <summary>
		/// Gets the BlockDataService with PPTable's UOW
		/// </summary>
		public DataServices.BlockDataService BlockDataService { get; private set; }

		#region Events
		/// <summary>
		/// Occurs when TaskEditor is supposed to open with the value of this Block
		/// </summary>
		public event Action<Editor.PPEditorBlock> EditBlockStarted;
		/// <summary>
		/// Occurs when this Block is supposed to be added to TaskEditor
		/// </summary>
		public event Action<Editor.PPEditorBlock> AddBlockToEditorStarted;
		/// <summary>
		/// Occurs when Job associated with this Block is supposed to be added to JobEditor
		/// </summary>
		public event Action<PPJobVm> AddJobToEditorStarted;
		/// <summary>
		/// Occurs when JobEditor is supposed to open with the value of Job associated with this Block
		/// </summary>
		public event Action<PPJobVm> EditJobStarted;
		/// <summary>
		/// Occurs when Report of this Block is supposed to be opened with ReportEditor
		/// </summary>
		public event Action<BlockVm> EditReportStarted;
		/// <summary>
		/// Occurs when this Block is supposed to be removed
		/// </summary>
		public event Action<BlockVm> DeleteBlockStarted;
		/// <summary>
		/// Occurs when the Job associated with this Block is supposed to be removed
		/// </summary>
		public event Action<PPJobVm> DeleteJobStarted;
		//public event Action<BlockVm, Action<DataServices.BlockDataService.InsertSetupBeforeBlockErrors>> InsertSetupStarted;
		#endregion

		#region Ctor, reload
		/// <summary>
		/// Creates an instance of BlockVm with the given model, parent and station index
		/// </summary>
		/// <param name="model"></param>
		/// <param name="parent"></param>
		/// <param name="stationIndex"></param>
        public BlockVm(Model.Block model, PPItemCollection parent, int stationIndex)
		{
			_model = model;
			BlockDataService = parent.BlockDataService;
            Parent = parent;
			RowIndex = stationIndex;
			StartDateTime = model.StartDateTime;
			DurationSeconds = model.DurationSeconds;
			initializeCommands();
		}

		/// <summary>
		/// Reloads current block full data from database (according to its id)
		/// <para></para>
		/// </summary>
		public void Reload()
		{
			var data = new Soheil.Core.PP.BlockFullData(BlockDataService, Id);
			_model = data.Model;
			Reload(data);
		}
		/// <summary>
		/// Reloads current blocks full data with the given <see cref="Soheil.Core.PP.BlockFullData"/>
		/// </summary>
		/// <param name="data">An instance of <see cref="Soheil.Core.PP.BlockFullData"/> filled with required data</param>
		public void Reload(Soheil.Core.PP.BlockFullData data)
		{
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
			//specify the job (if not null)
			if (data.Model.Job != null)
			{
				Job = new PPJobVm(data.Model.Job);
				//check if the SelectedJobId in PPTable is the same as this Job
				if (Parent.PPTable.SelectedJobId == Job.Id)
					IsJobSelected = true;
			}
			//add Tasks
			foreach (var task in data.Model.Tasks)
			{
				TaskList.Add(new PPTaskVm(task, this));
			}

			//check if the SelectedBlock in PPTable is the same as this block
			if (Parent.PPTable.SelectedBlock == null) ViewMode = PPViewMode.Simple;
			else ViewMode = (Parent.PPTable.SelectedBlock.Id == Id) ? PPViewMode.Report : PPViewMode.Simple;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the bindable containing parent of this Block
		/// </summary>
		public PPItemCollection Parent
		{
			get { return (PPItemCollection)GetValue(ParentProperty); }
			protected set { SetValue(ParentProperty, value); }
		}
		public static readonly DependencyProperty ParentProperty =
			DependencyProperty.Register("Parent", typeof(PPItemCollection), typeof(BlockVm), new UIPropertyMetadata(null));

		#region Product and State
		/// <summary>
		/// Gets the Product Id
		/// </summary>
		public int ProductId { get; protected set; }
		/// <summary>
		/// Gets a bindable Product Code
		/// </summary>
		public string ProductCode
		{
			get { return (string)GetValue(ProductCodeProperty); }
			protected set { SetValue(ProductCodeProperty, value); }
		}
		public static readonly DependencyProperty ProductCodeProperty =
			DependencyProperty.Register("ProductCode", typeof(string), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable Product Name
		/// </summary>
		public string ProductName
		{
			get { return (string)GetValue(ProductNameProperty); }
			protected set { SetValue(ProductNameProperty, value); }
		}
		public static readonly DependencyProperty ProductNameProperty =
			DependencyProperty.Register("ProductName", typeof(string), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable Product Color
		/// <para>Changing the value causes the ForeColorProperty to update</para>
		/// </summary>
		public Color ProductColor
		{
			get { return (Color)GetValue(ProductColorProperty); }
			protected set { SetValue(ProductColorProperty, value); }
		}
		public static readonly DependencyProperty ProductColorProperty =
			DependencyProperty.Register("ProductColor", typeof(Color), typeof(BlockVm), new UIPropertyMetadata(Colors.White, (d, e) =>
			d.SetValue(ForeColorProperty, new SolidColorBrush(((Color)e.NewValue).IsDark() ? Colors.White : Colors.Black))));
		public static readonly DependencyProperty ForeColorProperty =
			DependencyProperty.Register("ForeColor", typeof(SolidColorBrush), typeof(BlockVm), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));
		/// <summary>
		/// Gets a bindable State Code
		/// </summary>
		public string StateCode
		{
			get { return (string)GetValue(StateCodeProperty); }
			protected set { SetValue(StateCodeProperty, value); }
		}
		public static readonly DependencyProperty StateCodeProperty =
			DependencyProperty.Register("StateCode", typeof(string), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable value that indicates if this Block is doing work on a rework
		/// </summary>
		public bool IsRework
		{
			get { return (bool)GetValue(IsReworkProperty); }
			protected set { SetValue(IsReworkProperty, value); }
		}
		public static readonly DependencyProperty IsReworkProperty =
			DependencyProperty.Register("IsRework", typeof(bool), typeof(BlockVm), new UIPropertyMetadata(false));
		#endregion

		#region Block background texts
		/// <summary>
		/// Gets a bindable value for Block's TargetPoint
		/// </summary>
		public int BlockTargetPoint
		{
			get { return (int)GetValue(BlockTargetPointProperty); }
			protected set { SetValue(BlockTargetPointProperty, value); }
		}
		public static readonly DependencyProperty BlockTargetPointProperty =
			DependencyProperty.Register("BlockTargetPoint", typeof(int), typeof(BlockVm), new UIPropertyMetadata(0));
		/// <summary>
		/// Gets a bindable value for Block's produced count of grade 1 products
		/// </summary>
		public int BlockProducedG1
		{
			get { return (int)GetValue(BlockProducedG1Property); }
			protected set { SetValue(BlockProducedG1Property, value); }
		}
		public static readonly DependencyProperty BlockProducedG1Property =
			DependencyProperty.Register("BlockProducedG1", typeof(int), typeof(BlockVm), new UIPropertyMetadata(0));
		/// <summary>
		/// Gets a bindable value that shows the % of reports for this block that are filled
		/// <para>value is between 0 and 100</para>
		/// </summary>
		public string ReportFillPercent
		{
			get { return (string)GetValue(ReportFillPercentProperty); }
			protected set { SetValue(ReportFillPercentProperty, value); }
		}
		public static readonly DependencyProperty ReportFillPercentProperty =
			DependencyProperty.Register("ReportFillPercent", typeof(string), typeof(BlockVm), new UIPropertyMetadata("0"));
		/// <summary>
		/// Gets a bindable value that indicates if all reports for this Block are filleds
		/// </summary>
		public bool IsReportFilled
		{
			get { return (bool)GetValue(IsReportFilledProperty); }
			protected set { SetValue(IsReportFilledProperty, value); }
		}
		public static readonly DependencyProperty IsReportFilledProperty =
			DependencyProperty.Register("IsReportFilled", typeof(bool), typeof(BlockVm), new UIPropertyMetadata(false));
		#endregion

		#region Tasks,Job,Report
		/// <summary>
		/// Gets a bindable collection of tasks inside this Block
		/// </summary>
		public ObservableCollection<PPTaskVm> TaskList { get { return _taskList; } }
		private ObservableCollection<PPTaskVm> _taskList = new ObservableCollection<PPTaskVm>();

		/// <summary>
		/// Gets or sets a bindable value for the Job associated with this Block
		/// <para>If this task does not belong to any Job, this value is null</para>
		/// </summary>
		public PPJobVm Job
		{
			get { return (PPJobVm)GetValue(JobProperty); }
			set { SetValue(JobProperty, value); }
		}
		public static readonly DependencyProperty JobProperty =
			DependencyProperty.Register("Job", typeof(PPJobVm), typeof(BlockVm), new UIPropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value for the report of this Block
		/// </summary>
		public BlockReportVm BlockReport
		{
			get { return (BlockReportVm)GetValue(BlockReportProperty); }
			set { SetValue(BlockReportProperty, value); }
		}
		public static readonly DependencyProperty BlockReportProperty =
			DependencyProperty.Register("BlockReport", typeof(BlockReportVm), typeof(BlockVm), new UIPropertyMetadata(null));
		#endregion

		#region Other Props
		/// <summary>
		/// Gets a bindable value that indicates if a new Setup can be added before this Block
		/// </summary>
		public bool CanAddSetupBefore
		{
			get { return (bool)GetValue(CanAddSetupBeforeProperty); }
			set { SetValue(CanAddSetupBeforeProperty, value); }
		}
		public static readonly DependencyProperty CanAddSetupBeforeProperty =
			DependencyProperty.Register("CanAddSetupBefore", typeof(bool), typeof(BlockVm), new UIPropertyMetadata(true));
		/// <summary>
		/// Gets or sets a value that indicates if this Block is in Edit Mode
		/// </summary>
		public bool IsEditMode
		{
			get { return (bool)GetValue(IsEditModeProperty); }
			set { SetValue(IsEditModeProperty, value); }
		}
		public static readonly DependencyProperty IsEditModeProperty =
			DependencyProperty.Register("IsEditMode", typeof(bool), typeof(BlockVm), new UIPropertyMetadata(false));
		/// <summary>
		/// Gets or sets a value that indicates if the Job associated with this Block is selected
		/// <para>To use this you must set SelectedJobId on PPTable to this.Job.Id then reload this block</para>
		/// </summary>
		public bool IsJobSelected
		{
			get { return (bool)GetValue(IsJobSelectedProperty); }
			set { SetValue(IsJobSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsJobSelectedProperty =
			DependencyProperty.Register("IsJobSelected", typeof(bool), typeof(BlockVm),
			new UIPropertyMetadata(false));
		#endregion

		#endregion

		#region Commands
		/// <summary>
		/// Initializes all commands
		/// </summary>
		void initializeCommands()
		{
			ReloadBlockCommand = new Commands.Command(o => Reload());
			AddBlockToEditorCommand = new Commands.Command(o =>
			{
				try { if (AddBlockToEditorStarted != null) AddBlockToEditorStarted(new Editor.PPEditorBlock(_model)); }
				catch (Exception exp) { Message.AddEmbeddedException(exp.Message); }
			}, () => _model != null);
			EditItemCommand = new Commands.Command(o =>
			{
				try { if (EditBlockStarted != null) EditBlockStarted(new Editor.PPEditorBlock(_model)); }
				catch (Exception exp) { Message.AddEmbeddedException(exp.Message); }
			});
			AddJobToEditorCommand = new Commands.Command(o =>
			{
				try { if (AddJobToEditorStarted != null) AddJobToEditorStarted(Job); }
				catch (Exception exp) { Message.AddEmbeddedException(exp.Message); }
			}, () =>
			{
				if (Job == null) return false;
				if (Job.Id == 0) return false;
				return true;
			});
			EditJobCommand = new Commands.Command(o =>
			{
				try { if (EditJobStarted != null) EditJobStarted(Job); }
				catch (Exception exp) { Message.AddEmbeddedException(exp.Message); }
			}, () =>
			{
				if (Job == null) return false;
				if (Job.Id == 0) return false;
				return true;
			});
			EditReportCommand = new Commands.Command(o =>
			{
				try { if (EditReportStarted != null) EditReportStarted(this); }
				catch (Exception exp) { Message.AddEmbeddedException(exp.Message); }
			});
			DeleteItemCommand = new Commands.Command(o =>
			{
				try { if (DeleteBlockStarted != null) DeleteBlockStarted(this); }
				catch (Exception exp) { Message.AddEmbeddedException(exp.Message); }
			});
			DeleteJobCommand = new Commands.Command(o =>
			{
				try { if (DeleteJobStarted != null) DeleteJobStarted(Job); }
				catch (RoutedException exp)
				{
					if(exp.Target is PPTaskVm)
						(exp.Target as PPTaskVm).Message.AddEmbeddedException(exp.Message);
					else //if(exp.Target is BlockVm)
						Message.AddEmbeddedException(exp.Message);
				}
				catch (Exception exp) { Message.AddEmbeddedException(exp.Message); }
			}, () => { return Job != null; });
			InsertSetupBefore = new Commands.Command(async o =>
			{
				//the following part is async version of "var result = tmp.InsertSetupBeforeTask(Id)"
				var tmp = BlockDataService;
				var result = await Task.Run(() => tmp.InsertSetupBeforeBlock(Id));

				//in case of error callback with result
				if (result.IsSaved) Reload();
				else InsertSetupBeforeCallback(result);
				//if (InsertSetupStarted != null) InsertSetupStarted(this, InsertSetupBeforeCallback);
			});
		}
		/// <summary>
		/// This method runs after inserting a setup before this block
		/// </summary>
		/// <param name="result">result of BlockDataService's special operation, containing error messages</param>
		void InsertSetupBeforeCallback(DataServices.BlockDataService.InsertSetupBeforeBlockErrors result)
		{
			//exit if saved successfully
			if (result.IsSaved) return;
			//add the basic error message
			Message.AddEmbeddedException("قادر به افزودن آماده سازی نمی باشد.\nبرخی از Taskهای بعدی در این ایستگاه قابل تغییر نیستند.");
			foreach (var error in result.Errors)
			{
				switch (error.Item1)
				{
					case Soheil.Core.DataServices.BlockDataService.InsertSetupBeforeBlockErrors.ErrorSource.Task:
						var task = TaskList.FirstOrDefault(x => x.Id == error.Item3);
						if (task != null) task.Message.AddEmbeddedException(error.Item2);
						else Message.AddEmbeddedException(error.Item2);
						break;
					case Soheil.Core.DataServices.BlockDataService.InsertSetupBeforeBlockErrors.ErrorSource.NPT:
						var npt = Parent[this.RowIndex].NPTs.FirstOrDefault(x => x.Id == error.Item3);
						if (npt != null) npt.Message.AddEmbeddedException(error.Item2);
						else Message.AddEmbeddedException(error.Item2);
						break;
					case Soheil.Core.DataServices.BlockDataService.InsertSetupBeforeBlockErrors.ErrorSource.This:
						Message.AddEmbeddedException(error.Item2);
						break;
					default:
						break;
				}
			}
		}
		/// <summary>
		/// Gets a bindable command to reload this block
		/// </summary>
		public Commands.Command ReloadBlockCommand
		{
			get { return (Commands.Command)GetValue(ReloadBlockCommandProperty); }
			protected set { SetValue(ReloadBlockCommandProperty, value); }
		}
		public static readonly DependencyProperty ReloadBlockCommandProperty =
			DependencyProperty.Register("ReloadBlockCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable command to add this block to TaskEditor
		/// </summary>
		public Commands.Command AddBlockToEditorCommand
		{
            get { return (Commands.Command)GetValue(AddBlockToEditorCommandProperty); }
			protected set { SetValue(AddBlockToEditorCommandProperty, value); }
		}
        public static readonly DependencyProperty AddBlockToEditorCommandProperty =
            DependencyProperty.Register("AddBlockToEditorCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable command to add the Job associated with this block to JobEditor
		/// </summary>
		public Commands.Command AddJobToEditorCommand
		{
			get { return (Commands.Command)GetValue(AddJobToEditorCommandProperty); }
			protected set { SetValue(AddJobToEditorCommandProperty, value); }
		}
		public static readonly DependencyProperty AddJobToEditorCommandProperty =
			DependencyProperty.Register("AddJobToEditorCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable command to open the JobEditor with value of Job associated with this block
		/// </summary>
		public Commands.Command EditJobCommand
		{
			get { return (Commands.Command)GetValue(EditJobCommandProperty); }
			protected set { SetValue(EditJobCommandProperty, value); }
		}
		public static readonly DependencyProperty EditJobCommandProperty =
			DependencyProperty.Register("EditJobCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable command to delete the Job associated with this block
		/// </summary>
		public Commands.Command DeleteJobCommand
		{
			get { return (Commands.Command)GetValue(DeleteJobCommandProperty); }
			protected set { SetValue(DeleteJobCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteJobCommandProperty =
			DependencyProperty.Register("DeleteJobCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets a bindable command to insert a setup before this Block
		/// </summary>
		public Commands.Command InsertSetupBefore
		{
			get { return (Commands.Command)GetValue(InsertSetupBeforeProperty); }
			protected set { SetValue(InsertSetupBeforeProperty, value); }
		}
		public static readonly DependencyProperty InsertSetupBeforeProperty =
			DependencyProperty.Register("InsertSetupBefore", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		#endregion
	}
}
