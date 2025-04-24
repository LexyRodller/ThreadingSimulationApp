using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ThreadingSimulationApp.Models
{
    public interface IDeliveryService
    {
        Task DeliverGoodsAsync(ObservableCollection<Product> products);
    }
}