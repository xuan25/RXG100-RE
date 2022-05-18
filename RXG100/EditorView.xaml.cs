using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RXG100
{
    /// <summary>
    /// Interaction logic for EditorView.xaml
    /// </summary>
    public partial class EditorView : UserControl
    {
        public RXG100Plugin Plugin { get; private set; }

        public EditorView(RXG100Plugin plugin)
        {
            Plugin = plugin;
            DataContext = this;

            InitializeComponent();
        }

        public EditorView()
        {
            DataContext = this;

            InitializeComponent();
        }

        public ICommand AboutCommand { get; } = new AboutCommandHandler();

        public class AboutCommandHandler : ICommand
        {
            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                Splash splash = (Splash)parameter;
                splash.Visibility = Visibility.Visible;
            }
        }
    }

    public class NegativeFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1 - (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
