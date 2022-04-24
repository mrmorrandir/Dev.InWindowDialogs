using System;
using System.Threading;
using System.Threading.Tasks;
using MLib2.MVVM;

namespace Dev.InWindowDialogs.ViewModels;

/// <summary>
/// An abstract base class to implement dialog ViewModels that are used to show dialogs in the same window (via calling ViewModel) as the caller.
/// </summary>
/// <typeparam name="TDialogVMResultData">The return type of the data returned by the dialog. <seealso cref="IDialogVMResult{TDialogVMResultData}"/></typeparam>
public abstract class DialogVM<TDialogVMResultData> : ViewModelBase, IDialogVM<TDialogVMResultData>
{
    private CancellationTokenSource? _cancellationTokenSource;
    private CancellationTokenSource? _linkedTokenSource;
    private IDialogVMResult<TDialogVMResultData> _dialogResult = new DefaultDialogResult();
    private bool _isShown = false;

    protected void SetDialogResult(IDialogVMResult<TDialogVMResultData> dialogResult)
    {
        _dialogResult = dialogResult;
        _cancellationTokenSource?.Cancel();
    }
    
    /// <summary>
    /// "Shows" a dialog on another ViewModel and waits async for the user input, then returns the dialog result.
    /// </summary>
    /// <param name="show">A method to show the Dialog on another ViewModel (e.g. via property assignment)</param>
    /// <param name="cancellationToken">A token to cancel the dialog from the outside</param>
    /// <returns>A dialog result object</returns>
    /// <exception cref="TaskCanceledException"></exception>
    public async Task<IDialogVMResult<TDialogVMResultData>> ShowDialogResultAsync(Action<IDialogVM<TDialogVMResultData>> show, CancellationToken cancellationToken = default)
    {
        return await ShowDialogResultAsync(dialog =>
        {
            show(dialog);
            return Task.CompletedTask;
        }, cancellationToken);
    }

    /// <summary>
    /// "Shows" a dialog on another ViewModel and waits async for the user input, then returns the dialog result.
    /// </summary>
    /// <param name="showAsync">A async method to show the Dialog on another ViewModel (e.g. via property assignment)</param>
    /// <param name="cancellationToken">A token to cancel the dialog from the outside</param>
    /// <returns>A dialog result object</returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="TaskCanceledException"></exception>
    public async Task<IDialogVMResult<TDialogVMResultData>> ShowDialogResultAsync(Func<IDialogVM<TDialogVMResultData>, Task> showAsync, CancellationToken cancellationToken = default)
    {
        if (_isShown)
            throw new InvalidOperationException("The dialog is already shown.");
        _isShown = true;
        _cancellationTokenSource = new CancellationTokenSource();
        _linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cancellationTokenSource.Token);
        try
        {
            await showAsync(this);
            await Task.Delay(-1, _linkedTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
            if (cancellationToken.IsCancellationRequested)
                throw;
        }
        finally
        {
            _linkedTokenSource.Dispose();
            _linkedTokenSource = null;
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
            _isShown = false;
        }

        return _dialogResult;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _cancellationTokenSource?.Dispose();
        _linkedTokenSource?.Dispose();
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