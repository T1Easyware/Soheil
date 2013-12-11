using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Commands;

namespace Soheil.Core.Interfaces
{
    public interface IEntityObject : IEntity
    {
        AccessType Access { get; set; }

        Command SaveCommand { get; set; }

        void Save(object param);

        bool CanSave();
    }
}
