using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soheil.Core.ViewModels
{
	public class CausesVM : Interfaces.ISplitList
	{
		private Common.AccessType access;

		public CausesVM(Common.AccessType access)
		{
			// TODO: Complete member initialization
			this.access = access;
		}

		public Commands.Command AddCommand
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Commands.Command AddGroupCommand
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Commands.Command ViewCommand
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public System.Windows.Data.ListCollectionView Items
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public System.Windows.Data.ListCollectionView GroupItems
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Interfaces.ISplitContent CurrentContent
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void Add(object param)
		{
			throw new NotImplementedException();
		}

		public void AddGroup(object param)
		{
			throw new NotImplementedException();
		}

		public void View(object param)
		{
			throw new NotImplementedException();
		}

		public bool CanAdd()
		{
			throw new NotImplementedException();
		}

		public bool CanAddGroup()
		{
			throw new NotImplementedException();
		}

		public bool CanView()
		{
			throw new NotImplementedException();
		}
	}
	public class ActionPlanFishboneVM
	{
		private Model.FishboneNode_ActionPlan fishboneNodeActionPlan;
		private Common.AccessType access;
		private Common.RelationDirection relationDirection;

		public ActionPlanFishboneVM(Model.FishboneNode_ActionPlan fishboneNodeActionPlan, Common.AccessType access, Common.RelationDirection relationDirection)
		{
			// TODO: Complete member initialization
			this.fishboneNodeActionPlan = fishboneNodeActionPlan;
			this.access = access;
			this.relationDirection = relationDirection;
		}
	}
	public class FishboneNodeActionPlansVM : Interfaces.ISplitCollectionLink
	{
		private FishboneNodeVM fishboneNodeVM;
		private Common.AccessType Access;

		public FishboneNodeActionPlansVM(FishboneNodeVM fishboneNodeVM, Common.AccessType Access)
		{
			// TODO: Complete member initialization
			this.fishboneNodeVM = fishboneNodeVM;
			this.Access = Access;
		}

		public Interfaces.ISplitDetail Details
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Commands.Command ViewDetailsCommand
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void ViewDetails(object param)
		{
			throw new NotImplementedException();
		}

		public bool CanViewDetails()
		{
			throw new NotImplementedException();
		}

		public System.Windows.Visibility LinkVisibility
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Commands.Command ExcludeCommand
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Commands.Command IncludeCommand
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public System.Windows.Data.ListCollectionView AllItems
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void Exclude(object param)
		{
			throw new NotImplementedException();
		}

		public bool CanExclude()
		{
			throw new NotImplementedException();
		}

		public bool CanInclude()
		{
			throw new NotImplementedException();
		}

		public void Include(object param)
		{
			throw new NotImplementedException();
		}

		public void RefreshItems()
		{
			throw new NotImplementedException();
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
	}
}
