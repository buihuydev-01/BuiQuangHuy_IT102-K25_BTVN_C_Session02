using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SportsOddsApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // Show help dialog
            var helpMessage = @"HƯỚNG DẪN LẤY COOKIE:

1. Mở browser (Chrome/Edge) và vào trang web sports
2. Nhấn F12 để mở DevTools
3. Chọn tab 'Network'
4. Refresh trang (F5)
5. Tìm request 'today-data.aspx'
6. Click vào request đó
7. Tìm 'Request Headers' → 'Cookie:'
8. Copy toàn bộ giá trị của Cookie
9. Paste vào ô trên
10. Nhấn Enter hoặc click 'Làm mới'

LƯU Ý:
- Cookie sẽ hết hạn sau ~24h
- Cần cập nhật cookie mới khi app báo lỗi
- Không chia sẻ cookie của bạn

Ví dụ cookie:
ASP.NET_SessionId=xxx; _hjSession_xxx=...; fullScreenAds=true; ...";

            MessageBox.Show(helpMessage, "Hướng dẫn cập nhật Cookie", MessageBoxButton.OK, MessageBoxImage.Information);
            e.Handled = true;
        }
    }

    // Converter for odds colors
    public class OddsColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string oddsStr && double.TryParse(oddsStr, out double odds))
            {
                if (odds > 0)
                    return new SolidColorBrush(Colors.Red);
                else if (odds < 0)
                    return new SolidColorBrush(Colors.Blue);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // BooleanToVisibility converter
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
