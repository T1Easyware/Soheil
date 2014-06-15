using System.Collections.Generic;
using System.Collections.ObjectModel;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Interfaces;
using Soheil.Core.ViewModels;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class OrganizationChartPositionDataService : RecursiveDataServiceBase, IDataService<OrganizationChart_Position>
    {
        private readonly Repository<OrganizationChart_Position> _orgChatPositionRepository;
        
        public OrganizationChartPositionDataService(SoheilEdmContext context)
        {
            Context = context;
            _orgChatPositionRepository = new Repository<OrganizationChart_Position>(context);
        }
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public OrganizationChart_Position GetSingle(int id)
        {
                return _orgChatPositionRepository.FirstOrDefault(orgChartPosition => orgChartPosition.Id == id);
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<OrganizationChart_Position> GetAll()
        {
                IEnumerable<OrganizationChart_Position> entityList = _orgChatPositionRepository.GetAll();
                return new ObservableCollection<OrganizationChart_Position>(entityList);
        }

        /// <summary>
        /// Gets a list of view models representing currently active records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<OrganizationChart_Position> GetActives()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Adds the view model data to the model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public int AddModel(OrganizationChart_Position model)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Updates the view model data within the model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void UpdateModel(OrganizationChart_Position model)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Deletes view model data from the model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void DeleteModel(OrganizationChart_Position model)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// If exists, updates current model otherwise adds the new view model data to the database.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void AttachModel(OrganizationChart_Position model)
        {
            throw new System.NotImplementedException();
        }

        #region Overrides of RecursiveDataServiceBase

        public override ObservableCollection<IEntityNode> GetChildren(int id)
        {
            //return GetChildrenNodes(GetAll(), GetSingle(id));
            return null;
        }

        #endregion
    }
}
