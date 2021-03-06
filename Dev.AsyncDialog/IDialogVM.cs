using MLib2.MVVM;

namespace Dev.AsyncDialog;

public interface IDialogVM<TDialogVMResultData> : IViewModelBase, IDisposable
{
    Task<IDialogVMResult<TDialogVMResultData>> ShowDialogResultAsync(Action<IDialogVM<TDialogVMResultData>> show, CancellationToken cancellationToken = default);
    Task<IDialogVMResult<TDialogVMResultData>> ShowDialogResultAsync(Func<IDialogVM<TDialogVMResultData>, Task> showAsync, CancellationToken cancellationToken = default);
}