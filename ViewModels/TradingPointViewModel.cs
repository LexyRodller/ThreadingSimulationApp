using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ThreadingSimulationApp.Models;

namespace ThreadingSimulationApp.ViewModels
{
    public class TradingPointViewModel : INotifyPropertyChanged
    {
        private readonly TradingPoint? _tradingPoint;
        private string _status = "Not running";
        private string _lastEvent = string.Empty;

        public string Name => _tradingPoint?.Name ?? string.Empty;
        public ObservableCollection<Product>? Products => _tradingPoint?.Products;
        public ObservableCollection<Customer>? Customers => _tradingPoint?.Customers;
        public int TotalProductsCount => Products?.Count ?? 0;
        public ObservableCollection<string> EventLog { get; } = new();
        public ICommand RequestRemoveCommand { get; }

        public TradingPointViewModel(Action<TradingPointViewModel> removeAction)
        {
            RequestRemoveCommand = new RelayCommand(() => removeAction(this));
        }

        public TradingPointViewModel(TradingPoint tradingPoint, IDeliveryService deliveryService)
        {
            _tradingPoint = tradingPoint ?? throw new ArgumentNullException(nameof(tradingPoint));
            _tradingPoint.Products.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Products));
                OnPropertyChanged(nameof(TotalProductsCount));
            };
            _tradingPoint.DeliveryService = deliveryService ?? throw new ArgumentNullException(nameof(deliveryService));
            RequestRemoveCommand = new RelayCommand(() => { });

            _tradingPoint.ProductSold += OnProductSold;
            _tradingPoint.ProductOutOfStock += OnProductOutOfStock;
            _tradingPoint.DeliveryArrived += OnDeliveryArrived;
            _tradingPoint.DeliveryCompleted += OnDeliveryCompleted;
        }

        public string Status
        {
            get => _status;
            private set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public string LastEvent
        {
            get => _lastEvent;
            private set
            {
                _lastEvent = value;
                OnPropertyChanged();
            }
        }

        public void StartTrading()
        {
            if (_tradingPoint != null)
            {
                _tradingPoint.StartTrading();
                Status = "Trading";
                AddEvent("Trading started");
            }
        }

        public void StopTrading()
        {
            if (_tradingPoint != null)
            {
                _tradingPoint.StopTrading();
            }
        }

        private void OnProductSold(string message)
        {
            AddEvent(message);
            LastEvent = message;
            OnPropertyChanged(nameof(Products));
        }

        private void OnProductOutOfStock(string message)
        {
            Status = "Out of stock 1 product";
            AddEvent(message);
            LastEvent = message;
        }

        private void OnDeliveryArrived(string message)
        {
            AddEvent(message);
            LastEvent = message;
        }

        private void OnDeliveryCompleted(string message)
        {
            Status = "Trading";
            AddEvent(message);
            LastEvent = message;
            OnPropertyChanged(nameof(Products));
        }

        private void AddEvent(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            EventLog.Insert(0, $"[{timestamp}] {message}");

            if (EventLog.Count > 50)
            {
                EventLog.RemoveAt(EventLog.Count - 1);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}