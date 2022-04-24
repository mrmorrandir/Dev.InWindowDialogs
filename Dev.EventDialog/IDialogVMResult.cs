namespace Dev.EventDialog;

public interface IDialogVMResult<out TDialogVMResultData>
{
    bool Success { get; }
    TDialogVMResultData? Data { get; }
}