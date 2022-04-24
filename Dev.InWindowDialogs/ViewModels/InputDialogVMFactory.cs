namespace Dev.InWindowDialogs.ViewModels;

public class InputDialogVMFactory : IInputDialogVMFactory
{
    public IDialogVM<INameData> Create()
    {
        return new ManualInputDialogVM();
    }
}