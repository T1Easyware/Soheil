using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Core.Index;

namespace Soheil.Core.Index
{
	public class OeeRecordDetail : DependencyObject
	{
		public OeeRecordDetail()
		{
			SubRecords = new List<OeeRecordDetail>();
		}


		public double ParentHours { get; set; }
		public double Hours { get; set; }
		public string Text { get; set; }
		public List<OeeRecordDetail> SubRecords { get; private set; }
		public int ParentId { get; set; }
		public int Id { get; set; }
	}
}
