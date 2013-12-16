using Soheil.Core.Interfaces;

namespace Soheil.Core.ViewModels.InfoViewModels
{
    public interface IInfoViewModel : IViewModel
    {
        int Id { get; }
        string Code { get; set; }
        string Name { get; set; }
    }
}
