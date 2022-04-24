using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using System.Windows;
using MLib2.MVVM;

namespace Dev.InWindowDialogs.ViewModels;

public sealed class MainWindowVM : ViewModelBase
{
    private readonly IWelcomeScreen _welcomeScreen;
    private readonly IInputDialogVMFactory _inputDialogVMFactory;
    private IViewModelBase? _current = null;
    private bool _isStarted;
    private CancellationTokenSource? _cancellationTokenSource;

    public IViewModelBase? Current
    {
        get => _current;
        private set => SetProperty(ref _current, value);
    }
    
    public AsyncCommand StartAsyncCommand { get; }
    public DelegateCommand StopCommand { get; }
    public DelegateCommand ExitCommand { get; }
    
    public MainWindowVM(IWelcomeScreen welcomeScreen, IInputDialogVMFactory inputDialogVMFactory)
    {
        _welcomeScreen = welcomeScreen;
        _inputDialogVMFactory = inputDialogVMFactory;

        StartAsyncCommand = new AsyncCommand(StartAsync, errorCallback: StartError);
        StopCommand = new DelegateCommand(Stop, o => true);
        ExitCommand = new DelegateCommand(Exit, o => true);
        
        Current = _welcomeScreen;
    }

    private async Task StartAsync(object o)
    {
        if (_isStarted) 
            throw new InvalidOperationException("Der Prozess wurde bereits gestartet!");
        _isStarted = true;
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            using var inputDialog = _inputDialogVMFactory.Create();
            
            var inputDialogResult =
                await inputDialog.ShowDialogResultAsync(dialog =>
                {
                    Current = dialog;
                }, _cancellationTokenSource.Token);
            
            Current = _welcomeScreen;
            if (inputDialogResult.Success)
                MessageBox.Show(
                    $"The dialog result is {inputDialogResult.Success} and the name is {inputDialogResult.Data?.LastName}, {inputDialogResult.Data?.FirstName}!",
                    "Dialog Ergebnis", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show($"Der Start wurde abgebrochen.", "Abbruch", MessageBoxButton.OK,
                    MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Exception: {ex}");
        }

        _isStarted = false;
    }

    private void StartError(Exception ex)
    {
        MessageBox.Show($"Fehler beim Starten des Prozesses:\n{ex.Message}", "Fehler", MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    private void Stop(object o) => _cancellationTokenSource?.Cancel();

    private void Exit(object o)
    {
        Application.Current.Shutdown(0);
    }
}