using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;

namespace Soheil.Core.PP.Smart
{
	internal class SmartPlanner
	{
		DataServices.BlockDataService _blockDataService;
		DataServices.NPTDataService _nptDataService;
		DataServices.StationDataService _stationDataService;

		internal static string AiPath = @"C:\Users\Bizhan\Documents\AI\Planning\Debug";
		static string StationsPath { get { return Path.Combine(AiPath, "stations.txt"); } }
		static string JobsPath { get { return Path.Combine(AiPath, "jobs.txt"); } }
		//before running the ai all setups MUST be removed from edges of free spaces
		internal SmartPlanner()
		{
			initializeDataServices();
			createStationsFile();
		}
		void initializeDataServices()
		{
			var uow = new Dal.SoheilEdmContext();
			_blockDataService = new DataServices.BlockDataService(uow);
			_nptDataService = new DataServices.NPTDataService(uow);
			_stationDataService = new DataServices.StationDataService(uow);
		}
		void createStationsFile()
		{
			using (var file = new StreamWriter(StationsPath, false))
			{
				//counting stations
				var stations = _stationDataService.GetActives();
				file.WriteLine(stations.Count.ToString());

				//writing ids
				file.WriteLine(stations.Aggregate("", (line, model) => line += (model.Id.ToString() + " ")));

				//finding spaces
				foreach (var station in stations)
				{
					var spaces = new List<SmartSpace>();
					var blocks = _blockDataService.GetInRange(DateTime.Now, station.Id);
					var setups = _nptDataService.GetInRange(DateTime.Now, station.Id).OfType<Model.Setup>();
				}
			}
		}
	}
}
