using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP.PlannerAI
{
	public class Root
	{
		public int[,,] Changeovers { get; set; }
		public List<Job> Jobs { get; set; }
		public List<Product> Products { get; set; }
		public List<Row> Stations { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine(Products.Count.ToString());
			//products
			foreach (var product in Products)
			{
				//Number of Levels in Product i
				sb.AppendFormat("{0}\t", product.Levels.Length);
				foreach (var level in product.Levels)
				{
					//Number of States in Level J
					sb.AppendFormat("{0}\t", level.States.Count);
					foreach (var state in level.States)
					{
						//Number of StateStations,Requisites  in State K
						sb.AppendFormat("{0}#{1}\t", state.SSList.Count, state.Requisites.Count);
						//state stations
						foreach (var ss in state.SSList)
						{
							sb.AppendFormat("{0},{1}\t", ss.CycleTime, ss.StationIndex);
						}
						//requisites
						foreach (var ss in state.Requisites)
						{
							sb.AppendFormat("{0}?{1}\t", level.Index, state.Index);
						}
						//end of 1 state
					}
					//end of 1 level
				}
				//end of 1 product
				sb.AppendLine();
			}

			//PP
			sb.AppendLine(Stations.Count.ToString());
			foreach (var row in Stations)
			{
				sb.AppendFormat("{0}\t", row.Items.Count);
				//create one station
				foreach (var item in row.Items)
				{
					sb.AppendFormat("{0}\t{1}\t{2}\t", item.Start, item.End, item.IsFree ? 0 : 1);
					if (!item.IsFree)
					{
						sb.AppendFormat("{0}\t", item.Product.Index);
					}
				}
				sb.AppendLine();
			}


			//Changeovers
			for (int s = 0; s < Stations.Count; s++)
			{
				for (int i = 0; i < Products.Count; i++)
				{
					for (int j = 0; j < Products.Count; j++)
					{
						sb.AppendFormat("{0}\t", Changeovers[s, i, j]);
					}
					sb.AppendLine();
				}
			}

			//create jobs
			sb.AppendLine(Jobs.Count.ToString());
			foreach (var job in Jobs)
			{
				sb.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\n",
					job.Index,
					job.GroupId,
					job.Product.Index,
					job.Quantity,
					job.ReleaseTime,
					job.Deadline,
					job.Weight);
			}

			return sb.ToString();
		}
	}
}
