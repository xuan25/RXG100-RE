using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : UserControl
    {
        public Version CurrentVersion => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

        public Splash()
        {
            DataContext = this;
            InitializeComponent();

            MouseLeftButtonDown += Splash_MouseLeftButtonDown;
        }

        private void Splash_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        public ICommand NavigateCommand { get; } = new NavigateCommandHandler();

        public class NavigateCommandHandler : ICommand
        {
            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                Uri uri = (Uri)parameter;
                ProcessStartInfo processStartInfo = new ProcessStartInfo(uri.AbsoluteUri)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(processStartInfo);
            }
        }
    }
}
