using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using ThreadingSimulationApp.Models;

namespace ThreadingSimulationApp.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IDeliveryService _deliveryService = new DeliveryService();
        private int _tradingPointCounter = 1;
        private TradingPointViewModel? _selectedTradingPoint;

        public ObservableCollection<TradingPointViewModel> TradingPoints { get; } = new();
        public ICommand AddTradingPointCommand { get; }
        public ICommand RemoveTradingPointCommand { get; }

        public TradingPointViewModel? SelectedTradingPoint
        {
            get => _selectedTradingPoint;
            set
            {
                _selectedTradingPoint = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            AddTradingPointCommand = new RelayCommand(AddTradingPoint);
            RemoveTradingPointCommand = new RelayCommand(RemoveSelectedTradingPoint);
        }

        private void AddTradingPoint()
        {
            var name = $"Trading Point {_tradingPointCounter++}";
            var tradingPoint = new TradingPoint(name);
            
            var viewModel = new TradingPointViewModel(tradingPoint, _deliveryService);
            TradingPoints.Add(viewModel);
            viewModel.StartTrading();
            
            SelectedTradingPoint = viewModel;
        }

        private void RemoveSelectedTradingPoint()
        {
            if (SelectedTradingPoint != null)
            {
                SelectedTradingPoint.StopTrading();
                TradingPoints.Remove(SelectedTradingPoint);
                SelectedTradingPoint = null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}