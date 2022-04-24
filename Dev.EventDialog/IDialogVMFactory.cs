namespace Dev.EventDialog;

public interface IDialogVMFactory<TDialogVMResultData> 
{
    IDialogVM<TDialogVMResultData> Create();
}