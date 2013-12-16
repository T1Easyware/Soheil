using System;
using System.Collections.ObjectModel;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Dal;
using Soheil.Model;

namespace Soheil.Core.DataServices
{
    public class PositionAccessRuleDataService : IDataService<Position_AccessRule>
    {
        public event EventHandler<ModelAddedEventArgs<Position_AccessRule>> ModelUpdated;
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public Position_AccessRule GetSingle(int id)
        {
            Position_AccessRule entity;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Position_AccessRule>(context);
                entity = repository.Single(positionAccessRule => positionAccessRule.Id == id);
            }
            return entity;
        }

        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="accessRuleId">The accessRule id.</param>
        /// <param name="positionId">The position id</param>
        /// <returns></returns>
        public Position_AccessRule GetSingle(int positionId, int accessRuleId)
        {
            Position_AccessRule entity;
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Position_AccessRule>(context);
                entity = repository.FirstOrDefault(positionAccessRule => positionAccessRule.Position.Id == positionId && positionAccessRule.AccessRule.Id == accessRuleId, "Position","AccessRule", "AccessRule.Parent");
            }
            return entity;
        }

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Position_AccessRule> GetAll()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets a list of view models representing currently active records of the entity.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Position_AccessRule> GetActives()
        {
            throw new NotImplementedException();
        }

        public int AddModel(Position_AccessRule model)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateModel(Position_AccessRule model)
        {
            using (var context = new SoheilEdmContext())
            {
                var repository = new Repository<Position_AccessRule>(context);
                Position_AccessRule entity = repository.Single(positionAccessRule => positionAccessRule.Id == model.Id);

                context.Commit();
                if (ModelUpdated != null) ModelUpdated(this, new ModelAddedEventArgs<Position_AccessRule>(entity));
            }
        }

        public void DeleteModel(Position_AccessRule model)
        {
            throw new System.NotImplementedException();
        }

        public void AttachModel(Position_AccessRule model)
        {
            throw new System.NotImplementedException();
        }
    }
}