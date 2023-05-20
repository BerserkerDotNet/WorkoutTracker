using System.Globalization;

namespace WorkoutTracker.Services.Converters;

public class ImagePathConverter : IValueConverter
{
    CDNImageProvider _provider;

    public ImagePathConverter()
    {
        _provider = Application.Current.Handler.MauiContext.Services.GetRequiredService<CDNImageProvider>();
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