namespace Dev.ThreadEventDialog;

public interface IDialogVMFactory<TDialogVMResultData> 
{
    IDialogVM<TDialogVMResultData> Create();
}