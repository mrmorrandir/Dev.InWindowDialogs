using MLib2.MVVM;

namespace Dev.EventDialog;

public abstract class DialogVM<TDialogVMResultData> : ViewModelBase, IDialogVM<TDialogVMResultData>
{
    public event EventHandler<IDialogVMResult<TDialogVMResultData>>? Finished;

    protected void Finish(IDialogVMResult<TDialogVMResultData> result) => Finished?.Invoke(this, result);
}