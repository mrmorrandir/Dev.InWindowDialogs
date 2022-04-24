namespace Dev.InWindowDialogs.ViewModels.AsyncDialog;

public interface IDialogVMFactory<TDialogVMResultData> 
{
    IDialogVM<TDialogVMResultData> Create();
}