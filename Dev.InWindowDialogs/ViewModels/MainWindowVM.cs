using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using System.Windows;
using Dev.InWindowDialogs.ViewModels.EventDialog;
using Dev.InWindowDialogs.ViewModels.EventManualInputDialog;
using MLib2.MVVM;

namespace Dev.InWindowDialogs.ViewModels;

public sealed class MainWindowVM : ViewModelBase
{
    private readonly IWelcomeScreen _welcomeScreen;
    private readonly AsyncManualInputDialog.IInputDialogVMFactory _asyncInputDialogVMFactory;
    private readonly IInputDialogVMFactory _eventInputDialogVMFactory;
    private IViewModelBase? _current = null;
    private bool _isStarted;
    private bool _eventDialogIsStarted;
    private IDialogVM<INameData>? _eventDialog;
    private CancellationTokenSource? _cancellationTokenSource;

    public IViewModelBase? Current
    {
        get => _current;
        private set => SetProperty(ref _current, value);
    }
    
    public AsyncCommand StartAsyncDialogAsyncCommand { get; }
    public DelegateCommand StopAsyncDialogCommand { get; }
    public DelegateCommand StartEventDialogCommand { get; }
    public DelegateCommand StopEventDialogCommand { get; }
    
    public DelegateCommand ExitCommand { get; }
    
    public MainWindowVM(IWelcomeScreen welcomeScreen, AsyncManualInputDialog.IInputDialogVMFactory asyncInputDialogVMFactory, EventManualInputDialog.IInputDialogVMFactory eventInputDialogVMFactory)
    {
        _welcomeScreen = welcomeScreen;
        _asyncInputDialogVMFactory = asyncInputDialogVMFactory;
        _eventInputDialogVMFactory = eventInputDialogVMFactory;

        StartAsyncDialogAsyncCommand = new AsyncCommand(StartAsyncDialogAsync, errorCallback: StartAsyncDialogError);
        StopAsyncDialogCommand = new DelegateCommand(StopAsyncDialog, o => true);

        StartEventDialogCommand = new DelegateCommand(StartEventDialog, o => true);
        StopEventDialogCommand = new DelegateCommand(StopEventDialog, o => true);
        
        ExitCommand = new DelegateCommand(Exit, o => true);
        
        Current = _welcomeScreen;
    }

   
    #region EventDialog
    private void StartEventDialog(object obj)
    {
        if (_eventDialogIsStarted) 
            throw new InvalidOperationException("Der Prozess wurde bereits gestartet!");
        _eventDialogIsStarted = true;
            
        _eventDialog = _eventInputDialogVMFactory.Create();
        _eventDialog.Finished += DialogOnFinished;
        Current = _eventDialog;
    }
    
    private void StopEventDialog(object obj)
    {
        if (_eventDialog is not null)
            _eventDialog.Finished -= DialogOnFinished;
        _eventDialog = null;
        if (_eventDialogIsStarted)
            Current = _welcomeScreen;
        _eventDialogIsStarted = false;
    }

    private void DialogOnFinished(object? sender, IDialogVMResult<INameData> inputDialogResult)
    {
        try
        {
            (sender as IDialogVM<INameData>)!.Finished -= DialogOnFinished;
            Current = _welcomeScreen;
            if (inputDialogResult.Success)
                MessageBox.Show(
                    $"The dialog result is {inputDialogResult.Success} and the name is {inputDialogResult.Data?.LastName}, {inputDialogResult.Data?.FirstName}!",
                    "Dialog Ergebnis", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show($"Der Start wurde abgebrochen.", "Abbruch", MessageBoxButton.OK,
                    MessageBoxImage.Information);
        }
        finally
        {
            _eventDialogIsStarted = false;    
        }
    }
    #endregion

    private async Task StartAsyncDialogAsync(object o)
    {
        if (_isStarted) 
            throw new InvalidOperationException("Der Prozess wurde bereits gestartet!");
        _isStarted = true;
        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            using var inputDialog = _asyncInputDialogVMFactory.Create();
            
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
            MessageBox.Show($"Exception: {ex}", "Handled Exception from Dialog", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        _isStarted = false;
    }

    private void StartAsyncDialogError(Exception ex)
    {
        MessageBox.Show($"Fehler beim Starten des Prozesses:\n{ex.Message}", "Fehler", MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    private void StopAsyncDialog(object o)
    {
        _cancellationTokenSource?.Cancel();
        Current = _welcomeScreen;
    }

    private void Exit(object o)
    {
        Application.Current.Shutdown(0);
    }
}