using System;

namespace Soheil.Dal.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
		void PostponeChanges(object entity);
    }
}