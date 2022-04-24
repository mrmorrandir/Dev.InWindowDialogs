using MLib2.MVVM;

namespace Dev.ThreadEventDialog;

public interface IDialogVM<TDialogVMResultData> : IViewModelBase, IDisposable
{

    IDialogVMResult<TDialogVMResultData> ShowDialog(Action<IDialogVM<TDialogVMResultData>> show);
}