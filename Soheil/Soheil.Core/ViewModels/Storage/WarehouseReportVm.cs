using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Core.Printing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;

namespace Soheil.Core.ViewModels.Reports
{
	public class WarehouseReportVm : ViewModelBase, ISingularList
	{
        public WarehouseReportVm(AccessType access)
		{
			Access = access;

			#region init Commands


			RefreshCommand = new Command(Refresh, () => true);

			ChangeDayCommand = new Command(offset =>
			{
				StartDate = StartDate.AddDays((int)offset);
				LoadDailyReport();
			});

			#endregion

			_dataService = new WarehouseReportDataService();
			StartDate = DateTime.Now.AddDays(-1);
            EndDate = DateTime.Now;
			LoadDailyReport();
		}


		#region Properties
		public AccessType Access { get; set; }
		public Command RefreshCommand { get; set; }
		public Command ChangeDayCommand { get; set; }
		
		ReportData _reportData;
		WarehouseReportData _reports;

		WarehouseReportDataService _dataService;
		#endregion		

		#region DependencyProperties

		public FixedDocumentSequence Document
		{
			get { return (FixedDocumentSequence)GetValue(DocumentProperty); }
			set { SetValue(DocumentProperty, value); }
		}
		public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(FixedDocumentSequence), typeof(DailyReportVm), new PropertyMetadata(default(FixedDocumentSequence)));

	    public static readonly DependencyProperty StartDateProperty = DependencyProperty.Register(
	        "StartDate", typeof (DateTime), typeof (WarehouseReportVm), new PropertyMetadata(default(DateTime)));

	    public DateTime StartDate
	    {
	        get { return (DateTime) GetValue(StartDateProperty); }
	        set { SetValue(StartDateProperty, value); }
	    }

	    public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(
	        "EndDate", typeof (DateTime), typeof (WarehouseReportVm), new PropertyMetadata(default(DateTime)));

	    public DateTime EndDate
	    {
	        get { return (DateTime) GetValue(EndDateProperty); }
	        set { SetValue(EndDateProperty, value); }
	    }


		/// <summary>
		/// Gets or sets a bindable value that indicates Description
		/// </summary>
		public string Description
		{
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public static readonly DependencyProperty DescriptionProperty =
			DependencyProperty.Register("Description", typeof(string), typeof(DailyReportVm), new UIPropertyMetadata(""));

		/// <summary>
		/// Gets or sets a bindable collection of Shifts with supervisors
		/// </summary>


		#endregion

		#region Methods

		void Refresh(object param)
		{
			LoadDailyReport();
		}

		public void LoadDailyReport()
		{
			#region Init

		    _reports = _dataService.GetWarehouseReport(StartDate, EndDate);
			_reportData = new ReportData();

			var reportDocument = new ReportDocument();

			var reader =
				new StreamReader(new FileStream(@"Views\Reporting\WarehouseReportDocument.xaml", FileMode.Open, FileAccess.Read));
			reportDocument.XamlData = reader.ReadToEnd();
			reportDocument.XamlImagePath = Path.Combine(Environment.CurrentDirectory, @"Views\Reporting\");
			reader.Close();


			// set constant document values
			_reportData.ReportDocumentValues.Add("PrintDate", DateTime.Now); 
			#endregion


            var titleTabel = new DataTable("TitleTable");
            titleTabel.Columns.Add("ReportTitleName", typeof(string));
            titleTabel.Columns.Add("ReportTitleDate", typeof(string));
            var name = Common.Properties.Resources.ResourceManager.GetString("txtName") + _reports.Title;
            var date = DateTime.Now.ToPersianCompactDateTimeString();
            titleTabel.Rows.Add(new object[] { name, date });

            _reportData.DataTables.Add(titleTabel);

            var totalTabel = new DataTable("SummeryTable");
            totalTabel.Columns.Add("Total", typeof(string));

            totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalMatInv") + " : " + _reports.TotalStorage });
            totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalPrdInv") + " : " + _reports.TotalDischarge });
            totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotaProduction") + " : " + _reports.TotalProduction });
            totalTabel.Rows.Add(new object[] { Common.Properties.Resources.ResourceManager.GetString("txtTotalSale") + " : " + _reports.TotalSale });


            _reportData.DataTables.Add(totalTabel);

            var materialTable = new DataTable("MaterialsTable");

            materialTable.Columns.Add("Material", typeof(string));
            materialTable.Columns.Add("Warehouse", typeof(string));
            materialTable.Columns.Add("Inventory", typeof(string));
            materialTable.Columns.Add("Unit", typeof(string));

            foreach (var item in _reports.MaterialItems)
            {
                materialTable.Rows.Add( new object[] {
                        item.Material, item.Warehouse, item.Inventory,item.Unit
	                });
            }

            _reportData.DataTables.Add(materialTable);

            var productTable = new DataTable("ProductsTable");

            productTable.Columns.Add("Product", typeof(string));
            productTable.Columns.Add("Warehouse", typeof(string));
            productTable.Columns.Add("Inventory", typeof(string));
            productTable.Columns.Add("Production", typeof(string));
            productTable.Columns.Add("Price", typeof(string));
            productTable.Columns.Add("Fee", typeof(string));

            foreach (var item in _reports.ProductItems)
            {
                productTable.Rows.Add(new object[] {
                        item.Product, item.Warehouse, item.Inventory, item.Production, item.Price, item.Fee
	                });
            }

            _reportData.DataTables.Add(productTable);

           XpsDocument xps = reportDocument.CreateXpsDocument(_reportData);

            Document = xps.GetFixedDocumentSequence();
		}

		#endregion

	
	}
}
