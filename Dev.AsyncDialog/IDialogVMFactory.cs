namespace Dev.AsyncDialog;

public interface IDialogVMFactory<TDialogVMResultData> 
{
    IDialogVM<TDialogVMResultData> Create();
}