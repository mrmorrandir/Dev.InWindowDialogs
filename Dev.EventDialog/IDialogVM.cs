using MLib2.MVVM;

namespace Dev.EventDialog;

public interface IDialogVM<TDialogVMResultData> : IViewModelBase
{
    event EventHandler<IDialogVMResult<TDialogVMResultData>> Finished;
}