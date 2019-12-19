using System;

namespace VVMUI.Core.Command {
	public interface ICommand {
		event Action<bool> CanExecuteChanged;
		bool CanExecute ();
		void RefreshCanExecute ();
		void SetParameter (object parameter);
		void Execute ();
		Type GetEventDelegateType ();
	}

	public interface ICommand<T> {
		void GenericExecute (T arg);
	}
}