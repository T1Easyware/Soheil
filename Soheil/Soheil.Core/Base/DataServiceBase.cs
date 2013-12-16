using System;
using Soheil.Dal;
namespace Soheil.Core.Base
{
    public abstract class DataServiceBase : IDisposable
    {
		public SoheilEdmContext context { get; protected set; }

		public virtual void Dispose()
		{
			if (context != null)
				context.Dispose();
		}
	}
}