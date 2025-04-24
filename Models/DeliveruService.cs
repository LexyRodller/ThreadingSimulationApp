using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ThreadingSimulationApp.Models
{
    public class DeliveryService : IDeliveryService
    {
        public event Action<string>? DeliveryStarted;
        public event Action<string>? DeliveryCompleted;
        private readonly Random _random = new();

        public async Task DeliverGoodsAsync(ObservableCollection<Product> products)
        {
            var totalProducts = products.Count;
            DeliveryStarted?.Invoke($"Starting delivery of {totalProducts} products");
            
            await Task.Delay(_random.Next(1000, 3000));
            
            DeliveryCompleted?.Invoke($"Successfully delivered {totalProducts} products");
        }
    }
}