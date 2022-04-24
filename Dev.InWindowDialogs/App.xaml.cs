using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Dev.InWindowDialogs.ViewModels;
using Dev.InWindowDialogs.ViewModels.AsyncManualInputDialog;
using MediatR;
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
                    // TODO: And here is the second mediator problem!!! By registering all the NotificationHandlers etc. every time the mediator sends/publishes something the handler is newly created and therefore a new instance of the dialog parent is created - what is bullshit!
                    services.AddMediatR(Assembly.GetExecutingAssembly());
                    services.AddSingleton<IInputDialogVMFactory, InputDialogVMFactory>();
                    services
                        .AddSingleton<Dev.InWindowDialogs.ViewModels.EventManualInputDialog.IInputDialogVMFactory,
                            ViewModels.EventManualInputDialog.InputDialogVMFactory>();
                    services
                        .AddSingleton<Dev.InWindowDialogs.ViewModels.ThreadEventManualInputDialog.IInputDialogVMFactory,
                            Dev.InWindowDialogs.ViewModels.ThreadEventManualInputDialog.InputDialogVMFactory>();
                    services
                        .AddSingleton<Dev.InWindowDialogs.ViewModels.MediatorManualInputDialog.IInputDialogVMFactory,
                            Dev.InWindowDialogs.ViewModels.MediatorManualInputDialog.InputDialogVMFactory>();
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