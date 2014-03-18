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
		/// <summary>
		/// Gets the model of this block
		/// </summary>
		public Model.Block Model { get; protected set; }
		/// <summary>
		/// Gets Id property of the model representing this ViewModel
		/// </summary>
		public override int Id { get { return Model.Id; } }

		#region Ctor, reload
		/// <summary>
		/// Creates an instance of BlockVm with the given model, parent and station index
		/// </summary>
		/// <remarks>commands must be set after creating a block</remarks>
		/// <param name="model"></param>
		/// <param name="parent"></param>
		/// <param name="stationIndex"></param>
        public BlockVm(Model.Block model, PPItemCollection parent, int stationIndex)
		{
			Model = model;
            Parent = parent;
			RowIndex = stationIndex;
			StartDateTime = model.StartDateTime;
			DurationSeconds = model.DurationSeconds;
		}

		/// <summary>
		/// Reloads current blocks full data with the given <see cref="Soheil.Core.PP.BlockFullData"/>
		/// </summary>
		/// <param name="data">An instance of <see cref="Soheil.Core.PP.BlockFullData"/> filled with required data</param>
		public void Reload(Soheil.Core.PP.BlockFullData data)
		{
			Model = data.Model;

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
			}
			//add Tasks
			foreach (var task in data.Model.Tasks)
			{
				TaskList.Add(new PPTaskVm(task, this));
			}
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
		/// This method runs after inserting a setup before this block to declare the errors
		/// </summary>
		/// <param name="result">result of BlockDataService's special operation, containing error messages</param>
		internal void InsertSetupBeforeCallback(DataServices.BlockDataService.InsertSetupBeforeBlockErrors result)
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
		/// Gets or sets a bindable command to reload this block
		/// </summary>
		public Commands.Command ReloadBlockCommand
		{
			get { return (Commands.Command)GetValue(ReloadBlockCommandProperty); }
			set { SetValue(ReloadBlockCommandProperty, value); }
		}
		public static readonly DependencyProperty ReloadBlockCommandProperty =
			DependencyProperty.Register("ReloadBlockCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to add this block to TaskEditor
		/// </summary>
		public Commands.Command AddBlockToEditorCommand
		{
            get { return (Commands.Command)GetValue(AddBlockToEditorCommandProperty); }
			set { SetValue(AddBlockToEditorCommandProperty, value); }
		}
        public static readonly DependencyProperty AddBlockToEditorCommandProperty =
            DependencyProperty.Register("AddBlockToEditorCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to add the Job associated with this block to JobEditor
		/// </summary>
		public Commands.Command AddJobToEditorCommand
		{
			get { return (Commands.Command)GetValue(AddJobToEditorCommandProperty); }
			set { SetValue(AddJobToEditorCommandProperty, value); }
		}
		public static readonly DependencyProperty AddJobToEditorCommandProperty =
			DependencyProperty.Register("AddJobToEditorCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to open the JobEditor with value of Job associated with this block
		/// </summary>
		public Commands.Command EditJobCommand
		{
			get { return (Commands.Command)GetValue(EditJobCommandProperty); }
			set { SetValue(EditJobCommandProperty, value); }
		}
		public static readonly DependencyProperty EditJobCommandProperty =
			DependencyProperty.Register("EditJobCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to delete the Job associated with this block
		/// </summary>
		public Commands.Command DeleteJobCommand
		{
			get { return (Commands.Command)GetValue(DeleteJobCommandProperty); }
			set { SetValue(DeleteJobCommandProperty, value); }
		}
		public static readonly DependencyProperty DeleteJobCommandProperty =
			DependencyProperty.Register("DeleteJobCommand", typeof(Commands.Command), typeof(BlockVm), new UIPropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable command to insert a setup before this Block
		/// </summary>
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
