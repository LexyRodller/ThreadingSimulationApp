using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;
using System.Linq;

namespace ThreadingSimulationApp.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public static readonly BoolToBrushConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var status = value?.ToString();
            if (status == null || parameter == null)
                return Brushes.Transparent;

            var colorMappings = parameter.ToString()?
                .Split(';')
                .Select(mapping => mapping.Split(':'))
                .ToDictionary(
                    parts => parts[0].Trim(),
                    parts => parts.Length > 1 ? Color.Parse(parts[1].Trim()) : Colors.Transparent);

            foreach (var mapping in colorMappings)
            {
                if (status.Contains(mapping.Key))
                {
                    return new SolidColorBrush(mapping.Value);
                }
            }

            return colorMappings.TryGetValue("Default", out var defaultColor) 
                ? new SolidColorBrush(defaultColor) 
                : Brushes.Transparent;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}