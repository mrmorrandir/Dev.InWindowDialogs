# Dialogs in the same Window

From the code standpoint the Dialog with the TaskCompletionSource used instead of Task.Delay(-1, cancellationToken) is way more clean and understandable.

But in the end I really don't know if a dialog should be implemented with async/await  in this way. The alternatives are Events (like in the "good'ol" times) or Observable pattern (Reactive) which has a few advantages over the events but looks/works a littel more complicated in Code - and then there is the Mediator pattern with Notifications/Queries which looks complicated from the prozess flow.

## Async/Await

The process flow for this is like:
* ViewModel creates DialogViewModel
* ViewModel calls ``ShowDialogAsync(show, cancellationToken)``
* ``DialogViewModel.ShowDialogAsync`` creates ``TaskCompletionSource``, calls show and then **awaits** the ``TaskCompletionSource.Task``
* DialogViewModel commands will ``TrySetResult(dialogResult)`` and therefore finish the ``TaskCompletionSource.Task``
* ``DialogViewModel.ShowDialogAsync`` returns to ViewModel
* ViewModel does whatever it likes with the result and disposes DialogViewModel.

_When you did something like that in WinForms your could always reuse the Dialog - which you can also do here by calling ``ShowDialogAsync`` again.

## Events

The process flow for this is like:
* ViewModel creates DialogViewModel
* ViewModel attaches EventHandler to DialogViewModel
* ViewModel shows DialogViewModel

later (somewhere else? or in lambda?)

* DialogViewModel fires Event
* EventHandler detaches itself from DialogViewModel (don't forget!!)
* EventHandler does whatever it is designed to while the DialogViewModel waits for the execution to finish (is on the CallStack)

_Thinking about the process flow... you handle the event somewhere else... or better somewhere further up the CallStack, because you put that handle actions on top. I don't know if async/await does this in the background two, but it feels like it does this more like **ManualResetEvent/threadish** and returns back to original point where it was called. Async/Await fells more like the native Dialogs..._ 

## Mediator

Mediator would decouple everything, but the data flow would be:
* ViewModel implements ``Handler<DialogResult>``
* ViewModel creates DialogViewModel
* ViewModel shows DialogViewModel

later (somewhere else?)

* DialogViewModel sends/posts (?) Result
* The handler in the ViewModel is called (same execution context)
* The handler does whatever it is designed to while the DialogViewModel might wait for the execution to finish (is on the CallStack?) (Query - which seems unlikely, because the DialogViewModel does not need any data back from the handler).

_I might see an advantage here, because multiple handlers could get the dialog result to work with it - might try that..._ 
