using System;

namespace VVMUI.Core.Command {
	public interface ICommand {
		void NotifyCanExecute ();
		void AddCanExecuteChangedListener (Action handler);
		void RemoveCanExecuteChangedListener (Action handler);
		void BindVM (VMBehaviour vm);
		bool CanExecute (object parameter);
		void Execute (object parameter);
		object GetExecuteDelegate (object parameter);
	}

	public interface ICommand<T> {
		void Execute (T arg, object parameter);
	}
}