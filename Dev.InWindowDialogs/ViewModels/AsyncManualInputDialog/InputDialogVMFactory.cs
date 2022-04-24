using Dev.AsyncDialog;

namespace Dev.InWindowDialogs.ViewModels.AsyncManualInputDialog;

public class InputDialogVMFactory : IInputDialogVMFactory
{
    public IDialogVM<INameData> Create()
    {
        return new ManualInputDialogVM();
    }
}