using Dev.InWindowDialogs.ViewModels.EventDialog;

namespace Dev.InWindowDialogs.ViewModels.EventManualInputDialog;

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