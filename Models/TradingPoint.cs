using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingSimulationApp.Models
{
    public class TradingPoint
    {
        public string Name { get; }
        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();
        public ObservableCollection<Customer> Customers { get; } = new();
        public IDeliveryService? DeliveryService { get; set; }
        public bool IsRunning { get; private set; }
        private CancellationTokenSource? _cts;
        private readonly Random _random = new();

        public event Action<string>? ProductSold;
        public event Action<string>? ProductOutOfStock;
        public event Action<string>? DeliveryArrived;
        public event Action<string>? DeliveryCompleted;
    
        public TradingPoint(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            InitializeTradingPoint();
        }

        private void InitializeTradingPoint()
        {
            int productCount = _random.Next(3, 16);
            for (int i = 0; i < productCount; i++)
            {
                Products.Add(new Product($"Product {i + 1}", _random.Next(10, 100), _random.Next(1, 10)));
            }

            int customerCount = _random.Next(2, 21);
            for (int i = 0; i < customerCount; i++)
            {
                Customers.Add(new Customer($"Customer {i + 1}"));
            }
        }

        public void StartTrading()
        {
            if (IsRunning) return;
            
            IsRunning = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => TradingProcess(_cts.Token));
        }

        public void StopTrading()
        {
            IsRunning = false;
            _cts?.Cancel();
        }

        private async Task TradingProcess(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(2000);
                
                var outOfStockProducts = Products.Where(p => p.Quantity == 0).ToList();
                if (outOfStockProducts.Any())
                {
                    ProductOutOfStock?.Invoke($"Products out of stock: {string.Join(", ", outOfStockProducts.Select(p => p.Name))}");
                    await RequestDelivery(outOfStockProducts);
                }
                else if (Products.Count == 0)
                {
                    ProductOutOfStock?.Invoke("No products available! Waiting for delivery...");
                    await RequestDelivery();
                }

                if (_random.Next(0, 100) < 30 && Products.Any(p => p.Quantity > 0))
                {
                    int itemsToBuy = _random.Next(1, 4);
                    for (int i = 0; i < itemsToBuy; i++)
                    {
                        var availableProducts = Products.Where(p => p.Quantity > 0).ToList();
                        if (!availableProducts.Any()) break;
                        
                        var product = availableProducts[_random.Next(availableProducts.Count)];
                        var customer = Customers[_random.Next(Customers.Count)];
                        await customer.PurchaseProduct(this, product);
                        await Task.Delay(500);
                    }
                }
            }
        }

        public void SellProduct(Product product, Customer customer)
        {
            if (product.Quantity > 0)
            {
                product.Quantity--;
                ProductSold?.Invoke($"{customer.Name} bought {product.Name}. Remaining goods: {product.Quantity}");
                
                if (product.Quantity == 0)
                {
                    ProductOutOfStock?.Invoke($"Product {product.Name} is out of stock!");
                }
            }
        }

        private async Task RequestDelivery(List<Product>? productsToRestock = null)
        {
            if (DeliveryService == null) return;
    
            DeliveryArrived?.Invoke("Delivery arrived with products");
            
            if (productsToRestock != null)
            {
                foreach (var product in productsToRestock)
                {
                    product.Quantity = 10;
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    Products.Add(new Product($"Product {i + 1}", _random.Next(10, 100), 10));
                }
            }
            
            await DeliveryService.DeliverGoodsAsync(Products);
            DeliveryCompleted?.Invoke("Delivery completed. Products restocked");
        }
    }
}