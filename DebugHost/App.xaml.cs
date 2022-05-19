using AudioPlugSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace DebugHost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            string pluginName = e.Args[0];
            string playFile = null;

            if (e.Args.Length > 1)
            {
                playFile = e.Args[1];
            }

            IAudioPlugin audioPlugin = PluginLoader.LoadPluginFromAssembly(pluginName);

            MainWindow = new MainWindow(audioPlugin, playFile);
            MainWindow.Show();
        }
    }
}
