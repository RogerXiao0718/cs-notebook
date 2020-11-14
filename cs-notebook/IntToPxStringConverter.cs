using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace cs_notebook
{
    //參考 https://wpf-tutorial.com/data-binding/value-conversion-with-ivalueconverter/
    public class IntToPxStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return $"{value}px";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string str = value as string;
            int index = str.IndexOf("px");
            string pixelStr = str.Remove(index, 2);
            int pixel = 16;
            Int32.TryParse(pixelStr, out pixel);
            return pixel;
        }
    }
}
