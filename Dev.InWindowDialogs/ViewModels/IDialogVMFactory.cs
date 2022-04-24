namespace Dev.InWindowDialogs.ViewModels;

public interface IDialogVMFactory<TDialogVMResultData> 
{
    IDialogVM<TDialogVMResultData> Create();
}