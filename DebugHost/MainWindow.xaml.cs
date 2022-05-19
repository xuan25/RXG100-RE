using AudioPlugSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace DebugHost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IAudioPlugin plugin, string playFile)
        {
            InitializeComponent();

            // setup host
            VstHost host = new VstHost(plugin);
            if (playFile != null)
            {
                host.PlayAudio(playFile, true);
            }

            // setup editor view
            SizeToContent = SizeToContent.WidthAndHeight;
            UserControl editorView = ((dynamic)plugin).GetEditorView();

            editorView.Width = plugin.Editor.EditorWidth;
            editorView.Height = plugin.Editor.EditorHeight;

            Content = editorView;

            // unlock window size after loaded
            Loaded += (object sender, RoutedEventArgs e) =>
            {
                Task.Factory.StartNew(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        SizeToContent = SizeToContent.Manual;
                        editorView.Width = double.NaN;
                        editorView.Height = double.NaN;
                    });
                });
            };
        }
    }
}
