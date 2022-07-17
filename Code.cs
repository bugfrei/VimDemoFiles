using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TimeSpanConverter.ValueConverters
{
    public enum SpanFormat
    {
        Days = 1,
        Hours = 2,
        Minutes = 3,
        Seconds = 4
    }
    // Wenn es DependencyObject ist und die DependencyProperties registriert werden, funktioniert auch Binding... Theoretisch bis jetzt nur
    // public class TimeSpanConverter : DependencyObject, IValueConverter 
    public class TimeSpanConverter : DependencyObject, IValueConverter 
    {
        // Diese Properties kann man in der Definition des ValueConvertes in Window.Resource setzen (siehe XAML)
        public SpanFormat SpanFormat { get; set; }
        public string SpanFormatString { get; set; }
        public TimeSpanConverter()
        {
            SpanFormat = SpanFormat.Days;
            SpanFormatString = "0.0";
        }
        // Damit funktionieren zwar Bindings auf die Properties, aber eine Änderung im ModelView wird nicht aktualsiert :-(
        // Vielleicht werden ValueConverter nicht aktualisiert und nur bei der Erstellung der Resource einmalig initialisiert.
        //public static readonly DependencyProperty SpanFormatProperty = DependencyProperty.Register(nameof(SpanFormat), typeof(SpanFormat), typeof(TimeSpanConverter), new PropertyMetadata(null));
        //public static readonly DependencyProperty SpanFormatStringProperty = DependencyProperty.Register(nameof(SpanFormatString), typeof(string), typeof(TimeSpanConverter), new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan ts = (TimeSpan)value;
            if (ts == TimeSpan.MinValue)
            {
                return String.Empty;
            }
            switch (SpanFormat)
            {
                case SpanFormat.Days:
                    return ts.TotalDays.ToString(SpanFormatString);
                case SpanFormat.Hours:
                    return ts.TotalHours.ToString(SpanFormatString);
                case SpanFormat.Minutes:
                    return ts.TotalMinutes.ToString(SpanFormatString);
                case SpanFormat.Seconds:
                    return ts.TotalSeconds.ToString(SpanFormatString);
            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                int val;
                if (int.TryParse((string)value, out val))
                {
                    switch (SpanFormat)
                    {
                        case SpanFormat.Days:
                            return new TimeSpan(val, 0, 0, 0);
                        case SpanFormat.Hours:
                            return new TimeSpan(val, 0, 0);
                        case SpanFormat.Minutes:
                            return new TimeSpan(0, val, 0);
                        case SpanFormat.Seconds:
                            return new TimeSpan(0, 0, val);
                    }
                }
            }
            return new TimeSpan(0);
        }
    }
}
