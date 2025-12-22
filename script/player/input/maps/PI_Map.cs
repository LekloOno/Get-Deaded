using System.Collections;
using System.Collections.Generic;

namespace Pew;

public class PI_Map : IEnumerable<PI_InputAction>
{
    private readonly List<PI_InputAction> _inputActions;

    // Constructor allows initializing with a collection initializer
    public PI_Map()
    {
        _inputActions = [];
    }

    // Private Add method - works for collection initializers but is NOT accessible
    public void Add(PI_InputAction item) => _inputActions.Add(item);

    public void Enable() => _inputActions.ForEach(inputAction => inputAction.EnableAction());
    public void Disable() => _inputActions.ForEach(inputAction => inputAction.DisableAction());

    public IEnumerator<PI_InputAction> GetEnumerator() => _inputActions.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}