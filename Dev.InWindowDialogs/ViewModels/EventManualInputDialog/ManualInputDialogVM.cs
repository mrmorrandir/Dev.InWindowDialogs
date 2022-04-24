using Dev.EventDialog;
using MLib2.MVVM;

namespace Dev.InWindowDialogs.ViewModels.EventManualInputDialog;

public sealed class ManualInputDialogVM : DialogVM<INameData>
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;

    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }
    
    public DelegateCommand OkCommand { get; private set; }
    public DelegateCommand CancelCommand { get; private set; }
    
    public ManualInputDialogVM()
    {
        CreateCommandBinding();
    }

    public void Ok(object o)
    {
        Finish(new NameDataDialogResult(true, new NameData(_firstName, _lastName)));
    }

    public void Cancel(object o)
    {
        Finish(new NameDataDialogResult(false, null));
    }
}