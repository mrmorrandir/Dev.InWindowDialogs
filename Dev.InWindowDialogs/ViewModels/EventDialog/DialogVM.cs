using System;
using MLib2.MVVM;

namespace Dev.InWindowDialogs.ViewModels.EventDialog;

public interface IDialogVMResult<out TDialogVMResultData>
{
    bool Success { get; }
    TDialogVMResultData? Data { get; }
}

public interface IDialogVM<TDialogVMResultData> : IViewModelBase
{
    event EventHandler<IDialogVMResult<TDialogVMResultData>> Finished;
}

public abstract class DialogVM<TDialogVMResultData> : ViewModelBase, IDialogVM<TDialogVMResultData>
{
    public event EventHandler<IDialogVMResult<TDialogVMResultData>>? Finished;

    protected void Finish(IDialogVMResult<TDialogVMResultData> result) => Finished?.Invoke(this, result);
}

public interface IDialogVMFactory<TDialogVMResultData> 
{
    IDialogVM<TDialogVMResultData> Create();
}