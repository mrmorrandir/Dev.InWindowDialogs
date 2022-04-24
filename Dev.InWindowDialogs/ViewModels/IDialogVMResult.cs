namespace Dev.InWindowDialogs.ViewModels;

public interface IDialogVMResult<out TDialogVMResultData>
{
    bool Success { get; }
    TDialogVMResultData? Data { get; }
}