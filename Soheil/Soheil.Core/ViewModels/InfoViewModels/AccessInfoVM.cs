using System.Collections.ObjectModel;
using System.Windows.Data;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.Commands;
using Soheil.Core.Interfaces;
using Soheil.Model;

namespace Soheil.Core.ViewModels.InfoViewModels
{
    public class AccessInfoVM : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefectionInfoVM"/> class from the model.
        /// </summary>
        public AccessInfoVM()
        {
        }

        public int Id { get; set; }
        public ObservableCollection<AccessInfoVM> ChildNodes { get; set; }
        public int ParentId { get; set; }
        public string Code { get; set; }
        public AccessType Type { get; set; }
    }
}
