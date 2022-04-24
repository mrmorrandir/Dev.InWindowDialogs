using MLib2.MVVM;

namespace Dev.ThreadEventDialog;

public abstract class DialogVM<TDialogVMResultData> : ViewModelBase, IDialogVM<TDialogVMResultData>
{
    private readonly ManualResetEventSlim _dialogFinishedEvent = new ManualResetEventSlim(false);
    private IDialogVMResult<TDialogVMResultData> _dialogResult = new DefaultDialogResult();
    private bool _isShown = false;
    private CancellationTokenSource? _cancellationTokenSource;

    protected void SetDialogResult(IDialogVMResult<TDialogVMResultData> dialogResult)
    {
        _dialogResult = dialogResult;
        _dialogFinishedEvent.Set();
    }

    public IDialogVMResult<TDialogVMResultData> ShowDialog(Action<IDialogVM<TDialogVMResultData>> show)
    {
        if (_isShown)
            throw new InvalidOperationException("The dialog is already shown.");
        _isShown = true;
        _cancellationTokenSource = new CancellationTokenSource();
        try
        {
            show(this);
            // This blocks everything!!!
            _dialogFinishedEvent.Wait(_cancellationTokenSource.Token);
            return _dialogResult;
        }
        finally
        {
            _cancellationTokenSource.Dispose();
            _dialogFinishedEvent.Reset();
            _isShown = false;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _cancellationTokenSource?.Dispose();
        _dialogFinishedEvent?.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private class DefaultDialogResult : IDialogVMResult<TDialogVMResultData>
    {
        public bool Success => false;
        public TDialogVMResultData? Data => default;
    }
}