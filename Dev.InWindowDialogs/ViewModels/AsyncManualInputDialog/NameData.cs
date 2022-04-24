namespace Dev.InWindowDialogs.ViewModels.AsyncManualInputDialog;

public class NameData : INameData
{
    public string FirstName { get; }
    public string LastName { get; }
    
    public NameData(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}