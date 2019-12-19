using System;

namespace VVMUI.Core.Command {
	public interface ICommand {
		bool CanExecute ();
		void RefreshCanExecute ();
		event Action<bool> CanExecuteChanged;
		void Execute ();
		Type GetExecuteDelegateType ();
	}

	public interface ICommand<T> {
		void GenericExecute (T arg);
	}
}