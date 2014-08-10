using System.Data.Objects;
using Soheil.Dal.Interfaces;

namespace Soheil.Dal
{
    public class SoheilEdmContext : ObjectContext, IUnitOfWork
    {
		public SoheilEdmContext(string asd)
			: base(asd, "EdmContainer")
		{
			ContextOptions.LazyLoadingEnabled = true;
		}
		public SoheilEdmContext()
			: base("name=EdmContainer", "EdmContainer")
		{
			ContextOptions.LazyLoadingEnabled = true;
		}

        #region IUnitOfWork Members

        public void Commit()
        {
            SaveChanges();
			Soheil.Common.CommitNotifierHelper.Commit();
        }

        #endregion
    }
}