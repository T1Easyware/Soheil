using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soheil.Core.PP.Smart
{
	internal class SmartLayer : List<SmartStep>
	{
		internal SmartLayer(SmartStep startStep)
		{
			Add(startStep);
		}
		internal SmartLayer()
			: base()
		{

		}
		internal SmartLayer(IEnumerable<SmartStep> list)
			: base(list)
		{

		}
		internal void RemoveDuplicateStepsFrom(List<SmartLayer> uncleanedLayers)
		{
			foreach (var uncleanedLayer in uncleanedLayers)
			{
				foreach (var step in this)
				{
					//say S = state of this step
					//duplicateStep is step representing S
					var duplicateStep = uncleanedLayer.FirstOrDefault(x => x.State.Id == step.State.Id);
					if (duplicateStep != null)
					{
						//add those parents of duplicateStep to parents of this step, which are not already in this step's parents
						var parentsOfDuplicateStep = duplicateStep.ParentSteps.Where(x => !step.ParentSteps.Any(y => y.State.Id == x.State.Id));
						step.ParentSteps.AddRange(parentsOfDuplicateStep);
						//remove duplicateStep
						uncleanedLayer.Remove(duplicateStep);
					}
				}
			}
		}
		internal void RemoveDuplicateSteps()
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				//say S = State of this[i]
				//if more than one step represent S
				if (this.Count(x => x.State.Id == this[i].State.Id) > 1)
				{
					//add those parents of it to the first step representing S, which are not already in first step's parents
					var parentsOfFirst = this.First(x => x.State.Id == this[i].State.Id).ParentSteps;
					var parentsOfI = this[i].ParentSteps.Where(x => !parentsOfFirst.Any(y => y.State.Id == x.State.Id));
					parentsOfFirst.AddRange(parentsOfI);
					//then remove it
					this.RemoveAt(i);
					//since the loop is reversed, the first occurance of S will survive
				}
			}
		}
	}
}
