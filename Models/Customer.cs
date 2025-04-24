using System.Threading.Tasks;

namespace ThreadingSimulationApp.Models
{
    public class Customer
    {
        public string Name { get; }

        public Customer(string name)
        {
            Name = name;
        }

        public async Task PurchaseProduct(TradingPoint tradingPoint, Product product)
        {
            await Task.Delay(1000);
            tradingPoint.SellProduct(product, this);
        }

    }
}