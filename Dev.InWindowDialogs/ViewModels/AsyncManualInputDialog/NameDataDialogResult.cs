using Dev.InWindowDialogs.ViewModels.AsyncDialog;

namespace Dev.InWindowDialogs.ViewModels.AsyncManualInputDialog;

public class NameDataDialogResult : IDialogVMResult<INameData>
{
    public bool Success { get; }
    public INameData? Data { get; }
    
    public NameDataDialogResult(bool success, INameData? data)
    {
        Success = success;
        Data = data;
    }
}