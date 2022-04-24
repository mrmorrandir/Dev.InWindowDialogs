namespace Dev.MediatorDialog;

public interface IDialogVMFactory<TDialogVMResultData> 
{
    IDialogVM<TDialogVMResultData> Create();
}