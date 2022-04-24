using MLib2.MVVM;

namespace Dev.MediatorDialog;

public interface IDialogVM<TDialogVMResultData> : IViewModelBase
{
    void ShowDialog(Action<IDialogVM<TDialogVMResultData>> show);
}