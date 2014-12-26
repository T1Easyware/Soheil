using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using Soheil.Core.Commands;
using Soheil.Core.PP.PricingAI;
using Soheil.Core.Interfaces;
using Soheil.Common;

namespace Soheil.Core.ViewModels.PP.PricingAI
{
	public class PricingVm : DependencyObject, ISingularList
	{
		public event Action<PricingVm> MakeJobs;
		public AccessType Access { get; set; }
		public PricingVm(AccessType access)
		{
			Access = access;

			initializeCommands();
			//load MRU and AutoCloseConsole
			try
			{
				AutoCloseConsole = !Properties.Settings.Default.PPAI_IsDebug;
				OpenLastPrc = Properties.Settings.Default.PPAI_OpenLastPrc;
				var mru = Properties.Settings.Default.PPAI_MRU;
				if(mru!=null)
				{
					for (int i = 0; i < mru.Count; i++)
					{
						if (System.IO.File.Exists(mru[i]))
							MRU.Add(mru[i]);
					}
				}
			}
			catch { }
			if (Properties.Settings.Default.PPAI_MRU == null)
				Properties.Settings.Default.PPAI_MRU = new System.Collections.Specialized.StringCollection();

			//Create a new prc or open last used prc
			if (OpenLastPrc && MRU.Any())
				Open(MRU.First());
			else
				New();
		}

		#region Methods
		void AddToMRU(string path)
		{
			if (MRU.Contains(path)) MRU.Remove(path);
			if (Properties.Settings.Default.PPAI_MRU.Contains(path)) Properties.Settings.Default.PPAI_MRU.Remove(path);

			MRU.Insert(0, path);
			Properties.Settings.Default.PPAI_MRU.Insert(0, path);

			while (Properties.Settings.Default.PPAI_MRU.Count > 5)
				Properties.Settings.Default.PPAI_MRU.RemoveAt(5);
			Properties.Settings.Default.Save();
		}
		public void New()
		{
			Path = null;
			LoadData(new Root());
		}
		void Open()
		{
			OpenFileDialog dlg = new OpenFileDialog
			{
				DefaultExt = ".prc",
				Filter = "Pricing and Planning User Files|*.prc",
				RestoreDirectory = true
			};
			if (dlg.ShowDialog() == true)
			{
				try
				{
					Path = dlg.FileName;
					LoadData(Root.Load(dlg.FileName));
				}
				catch (Exception ex)
				{
					string msg = ex.Message;
					if (ex.InnerException != null)
						msg += ("\n" + ex.InnerException.Message);
					MessageBox.Show(msg);
				}
				AddToMRU(Path);
			}
		}
		void Open(string path)
		{
			if (!System.IO.File.Exists(path))
				MessageBox.Show("File not found.\n" + path);
			else
			{
				try
				{
					Path = path;
					LoadData(Root.Load(path));
				}
				catch (Exception ex)
				{
					string msg = ex.Message;
					if (ex.InnerException != null)
						msg += ("\n" + ex.InnerException.Message);
					MessageBox.Show(msg);
				}
				AddToMRU(path);
			}
		}
		void LoadData(Root data)
		{
			Params = new ParamsVm(data.Params);

			//Update VM Periods
			Periods.Clear();
			foreach (var period in data.Periods)
			{
				Periods.Add(new PeriodVm(period));
			}

			//Update VM Products
			var products = new List<ProductVm>();
			foreach (var product in data.Products)
			{
				var productVm = new ProductVm(product, Periods);
				products.Add(productVm);
			}
			Products = new ListCollectionView(products);
			if (Products.Count > 0)
				SelectedProduct = Products.GetItemAt(0) as ProductVm;
		}

		void Save()
		{
			try
			{
				//Update Data
				Root data = new Root(this);

				//Save Data
				if (Path == null) SaveAs();
				else data.Save();
			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				if (ex.InnerException != null)
					msg += ("\n" + ex.InnerException.Message);
				MessageBox.Show(msg);
			}
		}
		void SaveAs()
		{
			SaveFileDialog dlg = new SaveFileDialog
			{
				DefaultExt = ".prc",
				Filter = "Pricing and Planning User Files|*.prc",
				RestoreDirectory = true
			};
			if (dlg.ShowDialog() == true)
			{
				try
				{
					//Update Data
					Path = dlg.FileName;
					Root data = new Root(this);

					//Save Data
					data.Save(dlg.FileName);
				}
				catch (Exception ex)
				{
					string msg = ex.Message;
					if (ex.InnerException != null)
						msg += ("\n" + ex.InnerException.Message);
					MessageBox.Show(msg);
				}
				AddToMRU(Path);
			}
		}
		void SaveDataFile(string path = "data.txt")
		{
			var str = Root.ConvertToDataString(this);
			System.IO.StreamWriter wr = new System.IO.StreamWriter(path, false);
			wr.Write(str);
			wr.Close();
		}
		void SaveParamsFile(string path = "param.txt")
		{
			try
			{
				var str = Root.ConvertToParamsString(this);
				var wr = new System.IO.StreamWriter(path, false);
				wr.Write(str);
				wr.Close();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
		void Run()
		{
			if (System.IO.File.Exists("output.txt"))
				System.IO.File.Delete("output.txt");
			Results.Clear();
			var gus = new System.Diagnostics.Process();
			gus.StartInfo.FileName = "GUS.exe";
			gus.StartInfo.Arguments = "log.txt 1 data.txt param.txt";
			gus.EnableRaisingEvents = true;
			gus.Exited += gus_Exited;
			gus.Start();
		}

		/// <summary>
		/// Waits 10 seconds for IO to write the file and then fetches and shows output results
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void gus_Exited(object sender, EventArgs e)
		{
			int counter = 100;
			while (!System.IO.File.Exists("output.txt"))
			{
				if (--counter < 0) return;
				System.Threading.Thread.Sleep(100);
			}
			Dispatcher.Invoke(() => printResults());
		}
		void printResults()
		{
			try
			{
				ShowResults = true;

				//read all lines from output.txt
				var r = new System.IO.StreamReader("output.txt");
				var str = r.ReadToEnd().Split('\n', '\r');
				r.Close();
				
				//write all outputs as results
				foreach (var item in str)
				{
					var parts = item.Split('\t');
					if (parts.Length < 16) continue;

					//find the product's name
					int id = Convert.ToInt32(parts[0]);
					bool found = false;
					foreach (ProductVm product in Products)
					{
						if (product.Id == id)
						{
							//add the result row
							Results.Add(new ResultVm(product.Name, parts, id));
							found = true;
							break;
						}
					}
					if (!found)
						Results.Add(new ResultVm("[Id = " + id.ToString() + "]", parts, id));
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
		#endregion

		public string _path;
		public string Path
		{
			get { return _path; }
			private set
			{
				_path = value;
				if (value == null) FileName = "Not saved...";
				else
				{
					FileName = value.Split('\\').Last();
				}
			}
		}
		/// <summary>
		/// Gets or sets a bindable value that indicates FileName
		/// </summary>
		public string FileName
		{
			get { return (string)GetValue(FileNameProperty); }
			private set { SetValue(FileNameProperty, value); }
		}
		public static readonly DependencyProperty FileNameProperty =
			DependencyProperty.Register("FileName", typeof(string), typeof(PricingVm), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets a bindable value that indicates Products
		/// </summary>
		public ListCollectionView Products
		{
			get { return (ListCollectionView)GetValue(ProductsProperty); }
			set
			{
				SetValue(ProductsProperty, value);
				value.GroupDescriptions.Add(new PropertyGroupDescription("ProductGroupName"));
			}
		}
		public static readonly DependencyProperty ProductsProperty =
			DependencyProperty.Register("Products", typeof(ListCollectionView), typeof(PricingVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable collection that indicates Periods
		/// </summary>
		public ObservableCollection<PeriodVm> Periods { get { return _periods; } }
		private ObservableCollection<PeriodVm> _periods = new ObservableCollection<PeriodVm>();
		/// <summary>
		/// Gets or sets a bindable value that indicates SelectedProduct
		/// </summary>
		public ProductVm SelectedProduct
		{
			get { return (ProductVm)GetValue(SelectedProductProperty); }
			set { SetValue(SelectedProductProperty, value); }
		}
		public static readonly DependencyProperty SelectedProductProperty =
			DependencyProperty.Register("SelectedProduct", typeof(ProductVm), typeof(PricingVm), new PropertyMetadata(null));
		/// <summary>
		/// Gets or sets a bindable collection that indicates Results
		/// </summary>
		public ObservableCollection<ResultVm> Results { get { return _results; } }
		private ObservableCollection<ResultVm> _results = new ObservableCollection<ResultVm>();

		/// <summary>
		/// Gets or sets a bindable value that indicates ShowResults
		/// </summary>
		public bool ShowResults
		{
			get { return (bool)GetValue(ShowResultsProperty); }
			set { SetValue(ShowResultsProperty, value); }
		}
		public static readonly DependencyProperty ShowResultsProperty =
			DependencyProperty.Register("ShowResults", typeof(bool), typeof(PricingVm), new PropertyMetadata(false));
		/// <summary>
		/// Gets or sets a bindable value that indicates AutoCloseConsole
		/// </summary>
		public bool AutoCloseConsole
		{
			get { return (bool)GetValue(AutoCloseConsoleProperty); }
			set { SetValue(AutoCloseConsoleProperty, value); }
		}
		public static readonly DependencyProperty AutoCloseConsoleProperty =
			DependencyProperty.Register("AutoCloseConsole", typeof(bool), typeof(PricingVm), new PropertyMetadata(true, (d, e) =>
			{
				var vm = (PricingVm)d;
				Properties.Settings.Default.PPAI_IsDebug = !(bool)e.NewValue;
				Properties.Settings.Default.Save();
			}));
		/// <summary>
		/// Gets or sets a bindable value that indicates OpenLastPrc
		/// </summary>
		public bool OpenLastPrc
		{
			get { return (bool)GetValue(OpenLastPrcProperty); }
			set { SetValue(OpenLastPrcProperty, value); }
		}
		public static readonly DependencyProperty OpenLastPrcProperty =
			DependencyProperty.Register("OpenLastPrc", typeof(bool), typeof(PricingVm),
			new PropertyMetadata(true, (d, e) =>
			{
				var vm = (PricingVm)d;
				Properties.Settings.Default.PPAI_OpenLastPrc = (bool)e.NewValue;
				Properties.Settings.Default.Save();
			}));

		/// <summary>
		/// Gets or sets a bindable collection that indicates Most recent used files
		/// </summary>
		public ObservableCollection<string> MRU { get { return _mru; } }
		private ObservableCollection<string> _mru = new ObservableCollection<string>();


		/// <summary>
		/// Gets or sets a bindable value that indicates Params
		/// </summary>
		public ParamsVm Params
		{
			get { return (ParamsVm)GetValue(ParamsProperty); }
			set { SetValue(ParamsProperty, value); }
		}
		public static readonly DependencyProperty ParamsProperty =
			DependencyProperty.Register("Params", typeof(ParamsVm), typeof(PricingVm), new PropertyMetadata(null));


		#region Commands
		void initializeCommands()
		{
			NewCommand = new Command(o => New());
			OpenCommand = new Command(o => Open());
			OpenMRUCommand = new Command(param => Open((string)param));
			SaveCommand = new Command(o => Save());
			SaveAsCommand = new Command(o => SaveAs());

			SaveDataAsCommand = new Command(o =>
			{
				SaveFileDialog dlg = new SaveFileDialog
				{
					DefaultExt = ".txt",
					Filter = "Text Files|*.txt",
					RestoreDirectory = true
				};
				if (dlg.ShowDialog() == true)
				{
					try
					{
						SaveDataFile(dlg.FileName);
					}
					catch (Exception ex)
					{
						string msg = ex.Message;
						if (ex.InnerException != null)
							msg += ("\n" + ex.InnerException.Message);
						MessageBox.Show(msg);
					}
				}
			});
			SaveParamsAsCommand = new Command(o =>
			{
				SaveFileDialog dlg = new SaveFileDialog
				{
					DefaultExt = ".txt",
					Filter = "Text Files|*.txt",
					RestoreDirectory = true
				};
				if (dlg.ShowDialog() == true)
				{
					try
					{
						SaveParamsFile(dlg.FileName);
					}
					catch (Exception ex)
					{
						string msg = ex.Message;
						if (ex.InnerException != null)
							msg += ("\n" + ex.InnerException.Message);
						MessageBox.Show(msg);
					}
				}
			});
			SaveDataAndParamsCommand = new Command(o =>
			{
				SaveDataFile();
				SaveParamsFile();
				MessageBox.Show("Data file and Paramteres file are exported successfully.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
			});
			RunCommand = new Command(o =>
			{
				Save();
				SaveDataFile();
				SaveParamsFile();
				Run();
			});
			ExportJobsCommand = new Command(o =>
			{
				if (MakeJobs != null)
					MakeJobs(this);
			});
		}
		public Command NewCommand { get; set; }
		public Command OpenCommand { get; set; }
		public Command OpenMRUCommand { get; set; }
		public Command SaveCommand { get; set; }
		public Command SaveAsCommand { get; set; }
		public Command SaveDataAsCommand { get; set; }
		public Command SaveParamsAsCommand { get; set; }
		public Command SaveDataAndParamsCommand { get; set; }
		public Command RunCommand { get; set; }
		public Command PrintCommand { get; set; }
		public Command ExportJobsCommand { get; set; }
		public Command ReloadDataFileCommand { get; set; }
		public Command ReloadDatabaseCommand { get; set; }
		
		#endregion

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
	}
}
