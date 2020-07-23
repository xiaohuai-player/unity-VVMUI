using System;
using UnityEngine;

namespace VVMUI.Core.Command
{
    public class Vector2Command : BaseCommand<Vector2>
    {
        public Vector2Command(Func<object, bool> canExecuteHandler, Action<Vector2, object> executeHandler)
        {
            _canExecuteHandler = canExecuteHandler;
            _executeHandler = executeHandler;
        }
    }
}