using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using WorkoutTracker.MAUI.Services;

namespace WorkoutTracker.MAUI.Converters;

public class ImagePathConverter : IValueConverter
{
    CDNImageProvider _provider;

    public ImagePathConverter()
    {
        _provider = App.Current.ServiceProvider.GetRequiredService<CDNImageProvider>();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return _provider.GetFullPath(value.ToString());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}