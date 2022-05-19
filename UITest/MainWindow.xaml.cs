using RXG100;
using System;
using System.Collections.Generic;
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

namespace UITest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            RXG100Plugin plugin = new RXG100Plugin();
            plugin.Host = new Host();
            plugin.Initialize();

            SizeToContent = SizeToContent.WidthAndHeight;
            EditorView editorView = new EditorView(plugin)
            {
                Width = 1100,
                Height = 294,
            };

            Content = editorView;

            Loaded += (object sender, RoutedEventArgs e) =>
            {
                SizeToContent = SizeToContent.Manual;
                editorView.Width = double.NaN;
                editorView.Height = double.NaN;
            };
        }
    }
}
