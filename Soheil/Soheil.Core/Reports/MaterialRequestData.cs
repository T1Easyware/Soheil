using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.Reports
{
	public class MaterialRequestHour
	{
		public IEnumerable<MaterialRequestMaterial> Materials;
	}
	public class MaterialRequestMaterial
	{
		public Model.RawMaterial RawMaterial;
		public IEnumerable<MaterialRequestStation> Stations;
	}
	public class MaterialRequestStation
	{
		public Model.BOM Bom;
		public Model.Station Station;
		public double Quantity;
	}

}
