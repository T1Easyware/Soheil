using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soheil.Dal;

namespace Soheil.Core.Interfaces
{
    public interface IEntityCollection
    {
        SoheilEdmContext UnitOfWork { get; set; }
    }
}
