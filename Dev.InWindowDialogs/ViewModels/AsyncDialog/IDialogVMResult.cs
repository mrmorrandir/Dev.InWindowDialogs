namespace Dev.InWindowDialogs.ViewModels.AsyncDialog;

public interface IDialogVMResult<out TDialogVMResultData>
{
    bool Success { get; }
    TDialogVMResultData? Data { get; }
}