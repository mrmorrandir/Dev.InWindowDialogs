using Dev.MediatorDialog;
using MediatR;

namespace Dev.InWindowDialogs.ViewModels.MediatorManualInputDialog;

public class InputDialogVMFactory : IInputDialogVMFactory
{
    private readonly IMediator _mediator;

    public InputDialogVMFactory(IMediator mediator)
    {
        _mediator = mediator;
    }
    public IDialogVM<INameData> Create()
    {
        return new ManualInputDialogVM(_mediator);
    }
}