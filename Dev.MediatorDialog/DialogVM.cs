using MediatR;
using MLib2.MVVM;

namespace Dev.MediatorDialog;

public abstract class DialogVM<TDialogVMResultData> : ViewModelBase, IDialogVM<TDialogVMResultData>
{
    private readonly IMediator _mediator;

    protected DialogVM(IMediator mediator)
    {
        _mediator = mediator;
    }

    protected void SetDialogResult(IDialogVMResult<TDialogVMResultData> result)
    {
        var notification = new DialogResultNotification<TDialogVMResultData>(result);
        // TODO: Publish is async and is not awaited - Exceptions that are thrown inside the handlers are lost!
        _mediator.Publish(notification);
    }
    
    public void ShowDialog(Action<IDialogVM<TDialogVMResultData>> show)
    {
        show(this);
    }
}

public class DialogResultNotification<TDialogVMResultData> : IDialogVMResult<TDialogVMResultData>, INotification
{
    public DialogResultNotification(bool success, TDialogVMResultData? data)
    {
        Success = success;
        Data = data;
    }

    public DialogResultNotification(IDialogVMResult<TDialogVMResultData> result)
    {
        Success = result.Success;
        Data = result.Data;
    }

    public bool Success { get; }
    public TDialogVMResultData? Data { get; }
}