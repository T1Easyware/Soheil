using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.Reports
{
	public class MaterialPlanHour
	{
		public IEnumerable<MaterialPlanMaterial> Materials;
	}
	public class MaterialPlanMaterial
	{
		public Model.RawMaterial RawMaterial;
		public IEnumerable<MaterialPlanStation> Stations;
		public IEnumerable<Model.WarehouseTransaction> Transactions;
	}
	public class MaterialPlanStation
	{
		public Model.BOM Bom;
		public Model.Station Station;
		public double Quantity;
	}
}
