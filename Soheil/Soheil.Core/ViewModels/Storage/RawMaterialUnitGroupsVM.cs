using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.DataServices;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class RawMaterialUnitGroupsVM : ItemLinkViewModel
    {
        public RawMaterialUnitGroupsVM(RawMaterialVM rawMaterial, AccessType access):base(access)
        {
            UnitOfWork = new SoheilEdmContext();
            CurrentRawMaterial = rawMaterial;
            RawMaterialDataService = new RawMaterialDataService(UnitOfWork);
            RawMaterialDataService.UnitGroupAdded += OnUnitGroupAdded;
            RawMaterialDataService.UnitGroupRemoved += OnUnitGroupRemoved;
            UnitGroupDataService = new UnitGroupDataService(UnitOfWork);
            RawMaterialUnitGroupDataService = new RawMaterialUnitGroupDataService(UnitOfWork);

            var selectedVms = new ObservableCollection<RawMaterialUnitGroupVM>();
            foreach (var rawMaterialUnitGroup in RawMaterialDataService.GetUnitGroups(rawMaterial.Id))
            {
                selectedVms.Add(new RawMaterialUnitGroupVM(rawMaterialUnitGroup, Access, RawMaterialUnitGroupDataService, RelationDirection.Straight));
            }
            SelectedItems = new ListCollectionView(selectedVms);

            var allVms = new ObservableCollection<UnitGroupVM>();
            foreach (var unitGroup in UnitGroupDataService.GetActives())
            {
                allVms.Add(new UnitGroupVM(unitGroup, Access, UnitGroupDataService));
            }
            AllItems = new ListCollectionView(allVms);

            IncludeCommand = new Command(Include, CanInclude);
            ExcludeCommand = new Command(Exclude, CanExclude);
            IncludeRangeCommand = new Command(IncludeRange, CanIncludeRange);
            ExcludeRangeCommand = new Command(ExcludeRange, CanExcludeRange);
        }

        public RawMaterialVM CurrentRawMaterial { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public RawMaterialDataService RawMaterialDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public UnitGroupDataService UnitGroupDataService { get; set; }

        /// <summary>
        /// Gets or sets the data service.
        /// </summary>
        /// <value>
        /// The data service.
        /// </value>
        public RawMaterialUnitGroupDataService RawMaterialUnitGroupDataService { get; set; }



        private void OnUnitGroupRemoved(object sender, ModelRemovedEventArgs e)
        {
            foreach (RawMaterialUnitGroupVM  item in SelectedItems)
            {
                if (item.Id == e.Id)
                {
                    var model = UnitGroupDataService.GetSingle(item.UnitGroupId);
                    var returnedVm = new UnitGroupVM(model, Access, UnitGroupDataService);
                    AllItems.AddNewItem(returnedVm);
                    AllItems.CommitNew();
                    SelectedItems.Remove(item);
                    break;
                }
            }
        }

        private void OnUnitGroupAdded(object sender, ModelAddedEventArgs<RawMaterialUnitGroup> e)
        {
            var rawMaterialUnitGroupVm = new RawMaterialUnitGroupVM(e.NewModel, Access, RawMaterialUnitGroupDataService, RelationDirection.Straight);
            SelectedItems.AddNewItem(rawMaterialUnitGroupVm);
            SelectedItems.CommitNew();
            foreach (IEntityItem item in AllItems)
            {
                if (item.Id == rawMaterialUnitGroupVm.UnitGroupId)
                {
                    AllItems.Remove(item);
                    break;
                }
            }
        }

        public override void RefreshItems()
        {
            AllItems = new ListCollectionView(UnitGroupDataService.GetActives());
        }

        public override void Include(object param)
        {
            RawMaterialDataService.AddUnitGroup(CurrentRawMaterial.Id, ((IEntityItem) param).Id);
        }

        public override void Exclude(object param)
        {
            RawMaterialDataService.RemoveUnitGroup(CurrentRawMaterial.Id, ((IEntityItem) param).Id);
        }

        public override void IncludeRange(object param)
        {
            var tempList = new List<ISplitContent>();
            tempList.AddRange(AllItems.Cast<ISplitContent>());
            foreach (ISplitContent item in tempList)
            {
                if (item.IsChecked)
                {
                    RawMaterialDataService.AddUnitGroup(CurrentRawMaterial.Id, ((IEntityItem)item).Id);
                }
            }
        }

        public override void ExcludeRange(object param)
        {
            var tempList = new List<ISplitDetail>();
            tempList.AddRange(SelectedItems.Cast<ISplitDetail>());
            foreach (ISplitDetail item in tempList)
            {
                if (item.IsChecked)
                {
                    RawMaterialDataService.RemoveUnitGroup(CurrentRawMaterial.Id, ((IEntityItem)item).Id);
                }
            }
        }

    }
}