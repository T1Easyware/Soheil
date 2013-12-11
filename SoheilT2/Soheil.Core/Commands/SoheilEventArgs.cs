using System;

namespace Soheil.Core.Commands
{
	public class ModelAddedEventArgs<TModel> : EventArgs
	{
		public ModelAddedEventArgs(TModel newModel)
		{
			NewModel = newModel;
		}

		public TModel NewModel { get; set; }
	}

	public class ModelUpdatedEventArgs<TModel> : EventArgs
	{
		public ModelUpdatedEventArgs(TModel newModel, TModel oldModel)
		{
			NewModel = newModel;
			OldModel = oldModel;
		}

		public TModel NewModel { get; set; }
		public TModel OldModel { get; set; }
	}

	public class ModelRemovedEventArgs : EventArgs
	{
		public ModelRemovedEventArgs(int id)
		{
			Id = id;
		}

		public int Id { get; set; }
	}
}