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
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public OrganizationChart_Position GetSingle(int id)
        {
            OrganizationChart_Position model;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<OrganizationChart_Position>(context);
                model = repository.FirstOrDefault(orgChartPosition => orgChartPosition.Id == id);
            }
            return model;
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<OrganizationChart_Position> GetAll()
        {
            var models = new ObservableCollection<OrganizationChart_Position>();
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<OrganizationChart_Position>(context);
                IEnumerable<OrganizationChart_Position> entityList = repository.GetAll();
                models = new ObservableCollection<OrganizationChart_Position>(entityList);
            }
            return models;
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
