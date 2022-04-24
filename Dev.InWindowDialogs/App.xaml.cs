using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Dev.InWindowDialogs.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dev.InWindowDialogs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IInputDialogVMFactory, InputDialogVMFactory>();
                    services.AddSingleton<IWelcomeScreen, WelcomeScreenVM>();
                    services.AddSingleton<MainWindowVM>();
                })
                .Build();

            var mainWindow = new MainWindow
            {
                DataContext = _host.Services.GetRequiredService<MainWindowVM>()
            };
            mainWindow.Show();
        }
    }
}