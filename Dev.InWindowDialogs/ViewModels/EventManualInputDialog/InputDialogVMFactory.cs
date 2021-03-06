using Dev.EventDialog;

namespace Dev.InWindowDialogs.ViewModels.EventManualInputDialog;

public class InputDialogVMFactory : IInputDialogVMFactory
{
    public IDialogVM<INameData> Create()
    {
        return new ManualInputDialogVM();
    }
}