using System.Collections.ObjectModel;

namespace Soheil.Core.Interfaces
{
    /// <summary>
    /// Defines a method that provides access to the data.
    /// </summary>
    /// <typeparam name="TModel">The type of the view model.</typeparam>
    public interface IDataService<TModel>
    {
        /// <summary>
        /// Gets a single view model.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        TModel GetSingle(int id);

        /// <summary>
        /// Gets a list of view models representing all records of the entity.
        /// </summary>
        /// <returns></returns>
        ObservableCollection<TModel> GetAll();

        /// <summary>
        /// Gets a list of view models representing currently active records of the entity.
        /// </summary>
        /// <returns></returns>
        ObservableCollection<TModel> GetActives();

        /// <summary>
        /// Adds the model to the database.
        /// </summary>
        /// <param name="model">New model.</param>
        int AddModel(TModel model);
 
        /// <summary>
        /// Updates the model data within the database.
        /// </summary>
        /// <param name="model">Changed model.</param>
        void UpdateModel(TModel model);

        /// <summary>
        /// Deletes the model data from the database.
        /// </summary>
        /// <param name="model">The model to be removed.</param>
        void DeleteModel(TModel model);

        /// <summary>
        /// If exists, updates current model otherwise adds the new model to the database.
        /// </summary>
        /// <param name="model">The model to be attached.</param>
        void AttachModel(TModel model);

		/// <summary>
		/// Postpones all changes to the specified model for saving later (or discarding)
		/// </summary>
		/// <param name="model"></param>
		//???void Postpone(TModel model);

		/// <summary>
		/// Creates a clone for the current Generic Type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="model"></param>
		/// <returns></returns>
		//???T Clone(T model);
    }
}