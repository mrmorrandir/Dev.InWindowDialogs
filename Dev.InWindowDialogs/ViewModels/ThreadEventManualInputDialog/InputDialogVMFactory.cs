using Dev.ThreadEventDialog;

namespace Dev.InWindowDialogs.ViewModels.ThreadEventManualInputDialog;

public class InputDialogVMFactory : IInputDialogVMFactory
{
    public IDialogVM<INameData> Create()
    {
        return new ManualInputDialogVM();
    }
}