namespace Dev.MediatorDialog;

public interface IDialogVMResult<out TDialogVMResultData>
{
    bool Success { get; }
    TDialogVMResultData? Data { get; }
}