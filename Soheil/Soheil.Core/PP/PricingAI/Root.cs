using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Soheil.Core.ViewModels.PP.PricingAI;

namespace Soheil.Core.PP.PricingAI
{
	[Serializable]
	public class Root
	{
		public List<Product> Products { get; set; }
		public List<Period> Periods { get; set; }
		public Params Params { get; set; }

		[NonSerialized]
		public string Path;

		/// <summary>
		/// Creates a fresh instance of Root loaded with Database Products and 4 Periods
		/// </summary>
		public Root()
		{
			Path = null;
			Products = new List<Product>();
			Periods = new List<Period>()
			{
				new Period(0),
				new Period(1),
				new Period(2),
				new Period(3),
			};
			Params = new Params(new ParamsVm());

			//------------------
			//LOAD FROM DATABASE
			//------------------
			var ds = new DataServices.ProductDataService();
			var ps = ds.GetActives();
			foreach (var p in ps)
			{
				var product = new Product
				{
					Id = p.Id,
					Name = p.Name,
					GroupName = p.ProductGroup.Name,
					Inventory = p.ProductReworks.Sum(x=>x.Inventory),
					SpaceCoef = 1,//p.ProductReworks.Max(x=>x.sp),
				};
				foreach (var period in Periods)
				{
					product.Periods.Add(new ProductPeriod(product, period));
				}
				Products.Add(product);
			}
		}

		/// <summary>
		/// Creates an instance of Root loaded with values in given view model
		/// </summary>
		public Root(PricingVm vm)
		{
			Path = vm.Path;
			Products = new List<Product>();
			Periods = new List<Period>();
			Params = new Params(vm.Params);
			foreach (var periodVm in vm.Periods)
			{
				Periods.Add(new Period(periodVm));
			}
			foreach (ProductVm productVm in vm.Products)
			{
				var product = new Product(productVm);
				//foreach (var periodVm in vm.Periods)
				//{
				//	product.Periods.Add(new ProductPeriod(productVm, periodVm));
				//}
				Products.Add(product);
			}
		}

		//Deserialization constructor.
		public Root(SerializationInfo info, StreamingContext ctxt)
		{
			Products = (List<Product>)info.GetValue("Products", typeof(List<Product>));
			Periods = (List<Period>)info.GetValue("Periods", typeof(List<Period>));
			Params = (Params)info.GetValue("Params", typeof(Params));
		}

		//Serialization function.
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("Products", Products);
			info.AddValue("Periods", Periods);
			info.AddValue("Params", Params);
		}

		/// <summary>
		/// Saves a Root object to the specified Path
		/// </summary>
		/// <param name="path">Path of file to SaveAs to (pass null to Save)</param>
		public bool Save(string path = null)
		{
			//if parameter is null it's meant to Save
			if (path == null)
			{
				//same Path as before
				path = Path;
			}
			else//it's meant to SaveAs
			{
				//ask for overwrite
				/*if (File.Exists(path))
				{
					if (MessageBox.Show("A file with the same name already exists.\nDo you want to overwrite it?", "Overwrite",
						MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
						return false;
				}*/
				//new path to SaveAs to
				Path = path;
			}
			Stream stream = File.Open(path, FileMode.Create);
			BinaryFormatter bformatter = new BinaryFormatter();
			bformatter.Serialize(stream, this);
			stream.Close();
			return true;
		}
		/// <summary>
		/// Loads a Root object from the specified Path
		/// </summary>
		/// <param name="path">Path of file to Load from</param>
		/// <returns>Root object containing all serializable information</returns>
		public static Root Load(string path)
		{
			Stream stream = File.Open(path, FileMode.Open);
			BinaryFormatter bformatter = new BinaryFormatter();
			Root obj = (Root)bformatter.Deserialize(stream);
			stream.Close();

			obj.Path = path;
			return obj;
		}

		internal static string ConvertToParamsString(PricingVm vm)
		{
			StringBuilder builder = new StringBuilder();
			Root root = new Root(vm);
			root.Products.RemoveAll(x => x.Periods.All(y => y.MaxProduction == 0) || x.Periods.All(y => !y.Prices.Any()));

			builder.Append("D2.Debug=");
			builder.Append(vm.AutoCloseConsole ? 0 : 1);
			builder.AppendLine();
	
			builder.AppendLine("//Global");

			builder.Append("G1.maxRuns=");
			builder.Append(root.Params.maxRuns);
			builder.AppendLine();
			builder.Append("G3.maxDfss=");
			builder.Append(root.Params.maxDfss);
			builder.AppendLine();
			builder.Append("G4.idleCount=");
			builder.Append(root.Params.idleCount);
			builder.AppendLine();
			builder.Append("G5.timeLimit=");
			builder.Append(root.Params.timeLimit);
			builder.AppendLine();
			builder.Append("G6.loggingMode=1");
			builder.AppendLine();

			builder.AppendLine("//Algorithm");

			builder.Append("A2.mmSize=");
			builder.Append(root.Params.mmSize);
			builder.AppendLine();
			builder.Append("A3.maxInitPop=");
			builder.Append(root.Params.maxInitPop);
			builder.AppendLine();

			builder.Append("P1.memorySize=");
			builder.Append(root.Params.memorySize);
			builder.AppendLine();
			builder.Append("P2.initPixValue=0.1");
			builder.AppendLine();
			builder.Append("P4.translationFunction=");
			builder.Append(root.Params.translationFunction+1);
			builder.AppendLine();
			builder.Append("P5.sigmoidCoef=1.1461");
			builder.AppendLine();

			return builder.ToString();
		}
		internal static string ConvertToDataString(PricingVm vm)
		{
			//filter active products
			var products = new List<Product>();
			foreach (ProductVm productVm in vm.Products)
			{
				if (productVm.IsActive)
				{
					//this orders periods by their indices
					var product = new Product(productVm);
					products.Add(product);
				}
			}

			//build file
			StringBuilder builder = new StringBuilder();

			//path
			builder.AppendLine(vm.Path);
			
			//periods,products count
			builder.AppendFormat("{0},{1}", vm.Periods.Count, products.Count);
			builder.AppendLine();

			//periods
			foreach (var periodVm in vm.Periods.OrderBy(x => x.Index))
			{
				builder.AppendFormat("{0}\t{1}\t{2}", periodVm.Duration, periodVm.TotalCapacity, periodVm.TotalBudget);
				builder.AppendLine();
			}

			//products
			foreach (var productVm in products)
			{
				builder.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t",
					productVm.Id,
					productVm.FinishedCost,
					productVm.InventoryCost,
					productVm.LostSaleCost,
					productVm.Inventory,
					productVm.SpaceCoef);
				//productPeriods
				foreach (var pp in productVm.Periods)//it is already sorted
				{
					builder.AppendFormat("{0}\t{1}\t", pp.MaxProduction, pp.Prices.Count);
					foreach (var price in pp.Prices)
					{
						builder.AppendFormat("{0}\t{1}\t{2}\t", price.Fee, price.MinDemand, price.MaxDemand);
					}
				}
				builder.AppendLine();
			}

			return builder.ToString();
		}
	}
}
