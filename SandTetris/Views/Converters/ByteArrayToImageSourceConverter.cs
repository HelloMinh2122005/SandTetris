using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace SandTetris.Views.Converters;

public class ByteArrayToImageSourceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is byte[] byteArray && byteArray.Length > 0)
        {
            return ImageSource.FromStream(() => new MemoryStream(byteArray));
        }

        // i should handle this thing if the value is null
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // this thing could be null also, hopefully not 
        throw new NotImplementedException();
    }
}
