using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace ThreadingSimulationApp.Models
{
    public class Product : INotifyPropertyChanged
    {
        public string Name { get; }
        public decimal Price { get; }
        public DateTime ProductionDate { get; }
        private int _quantity;
    
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                }
            }
        }

        public Product(string name, decimal price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
            ProductionDate = DateTime.Now;
        }

        public override string ToString() => $"{Name} - {Price:C} (Qty: {Quantity}, produced at {ProductionDate:HH:mm:ss})";
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}