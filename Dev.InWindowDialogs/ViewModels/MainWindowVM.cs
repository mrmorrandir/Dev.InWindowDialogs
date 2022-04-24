using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using System.Windows;
using Dev.MediatorDialog;
using MediatR;
using MLib2.MVVM;

namespace Dev.InWindowDialogs.ViewModels;

public sealed class MainWindowVM : ViewModelBase, INotificationHandler<DialogResultNotification<MediatorManualInputDialog.INameData>>
{
    private readonly Guid _guid = Guid.NewGuid();
    private readonly IWelcomeScreen _welcomeScreen;
    private readonly AsyncManualInputDialog.IInputDialogVMFactory _asyncInputDialogVMFactory;
    private readonly EventManualInputDialog.IInputDialogVMFactory _eventInputDialogVMFactory;
    private readonly ThreadEventManualInputDialog.IInputDialogVMFactory _threadInputDialogVMFactory;
    private readonly MediatorManualInputDialog.IInputDialogVMFactory _mediatorInputDialogVMFactory;
    private IViewModelBase? _current = null;
    private bool _isStarted;
    private bool _eventDialogIsStarted;
    private Dev.EventDialog.IDialogVM<EventManualInputDialog.INameData>? _eventDialog;
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
    public DelegateCommand StartThreadEventDialogCommand { get; }
    public DelegateCommand StopThreadEventDialogCommand { get; }

    public DelegateCommand StartMediatorDialogCommand { get; }
    public DelegateCommand ExitCommand { get; }

    public MainWindowVM(IWelcomeScreen welcomeScreen,
        AsyncManualInputDialog.IInputDialogVMFactory asyncInputDialogVMFactory,
        EventManualInputDialog.IInputDialogVMFactory eventInputDialogVMFactory,
        ThreadEventManualInputDialog.IInputDialogVMFactory threadInputDialogVMFactory,
        MediatorManualInputDialog.IInputDialogVMFactory mediatorInputDialogVMFactory)
    {
        _welcomeScreen = welcomeScreen;
        _asyncInputDialogVMFactory = asyncInputDialogVMFactory;
        _eventInputDialogVMFactory = eventInputDialogVMFactory;
        _threadInputDialogVMFactory = threadInputDialogVMFactory;
        _mediatorInputDialogVMFactory = mediatorInputDialogVMFactory;

        StartAsyncDialogAsyncCommand = new AsyncCommand(StartAsyncDialogAsync, errorCallback: StartAsyncDialogError);
        StopAsyncDialogCommand = new DelegateCommand(StopAsyncDialog, o => true);

        StartEventDialogCommand = new DelegateCommand(StartEventDialog, o => true);
        StopEventDialogCommand = new DelegateCommand(StopEventDialog, o => true);

        StartThreadEventDialogCommand = new DelegateCommand(StartThreadEventDialog, o => true);
        StopThreadEventDialogCommand = new DelegateCommand(StopThreadEventDialog, o => true);

        StartMediatorDialogCommand = new DelegateCommand(StartMediatorDialog, o => true);
        ExitCommand = new DelegateCommand(Exit, o => true);

        Current = _welcomeScreen;
    }

    private void StartMediatorDialog(object obj)
    {
        Debug.WriteLine($"MediatorDialog Caller {_guid}");
        _mediatorInputDialogVMFactory.Create().ShowDialog(dialog => Current = dialog);
    }
    
    public Task Handle(DialogResultNotification<MediatorManualInputDialog.INameData> inputDialogResult, CancellationToken cancellationToken)
    {
        // Handling takes place in a new Instance of MainWindowVM... intriguing because in the DI it is registered as Singleton.
        // Looks like MediatR does not use the DI container to look for Handlers, but is Scanning the Assembly and then - with the help
        // of the DI ServiceProvider spawning new Handlers instead. (Maybe ActivatorUtilities.CreateInstance(...) instead of sp.GetRequiredService<...>()?!)
        Debug.WriteLine($"MediatorDialog Handler {_guid}");
        try
        {
            // Because Current is part of the new MainWindowVM instance that was created by the Mediator you will see nothing.
            // You're completely somewhere else!
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
            MessageBox.Show($"Exception in Mediator Notification Handler!\n{ex}", "Exception", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        return Task.CompletedTask;
    }
    

    private void StartThreadEventDialog(object obj)
    {
        // This does not work - because everything is blocked because it's all sync
        var inputDialogResult = _threadInputDialogVMFactory.Create().ShowDialog(dialog => Current = dialog);
        Current = _welcomeScreen;
        
        if (inputDialogResult.Success)
            MessageBox.Show(
                $"The dialog result is {inputDialogResult.Success} and the name is {inputDialogResult.Data?.LastName}, {inputDialogResult.Data?.FirstName}!",
                "Dialog Ergebnis", MessageBoxButton.OK, MessageBoxImage.Information);
        else
            MessageBox.Show($"Der Start wurde abgebrochen.", "Abbruch", MessageBoxButton.OK,
                MessageBoxImage.Information);
    }
    
    private void StopThreadEventDialog(object obj)
    {
        // No Idea what to do... but it's sync so no chance!
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

    private void DialogOnFinished(object? sender, Dev.EventDialog.IDialogVMResult<EventManualInputDialog.INameData> inputDialogResult)
    {
        try
        {
            (sender as Dev.EventDialog.IDialogVM<EventManualInputDialog.INameData>)!.Finished -= DialogOnFinished;
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