using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VVMUI.Core;
using VVMUI.Core.Command;
using VVMUI.Core.Data;

namespace VVMUI.Core.Test {
	public class BaseCommandBindingTestVMBehaviour : VMBehaviour {
		public bool BtnInteractable = false;
		[SerializeField] private bool _btnClicked = false;
		public bool BtnClicked {
			get {
				return _btnClicked;
			}
			set {
				_btnClicked = false;
			}
		}
		public ButtonCommand BtnCommand;
		public bool BtnCommand_CanExecute (object parameter) {
			return BtnInteractable;
		}
		public void BtnCommand_Execute (object parameter) {
			_btnClicked = true;
		}

		public bool ToggleInteractable = false;
		[SerializeField] private bool _toggleIsOn = false;
		public bool ToggleIsOn {
			get {
				return _toggleIsOn;
			}
		}
		public BoolCommand TglCommand;
		public bool TglCommand_CanExecute (object parameter) {
			return ToggleInteractable;
		}
		public void TglCommand_Execute (bool value, object parameter) {
			_toggleIsOn = value;
		}

		public bool SliderInteractable = false;
		[SerializeField] private float _sliderValue = -1.0f;
		public float SliderValue {
			get {
				return _sliderValue;
			}
		}
		public FloatCommand SliderCommand;
		public bool SliderCommand_CanExecute (object parameter) {
			return SliderInteractable;
		}
		public void SliderCommand_Execute (float value, object parameter) {
			_sliderValue = value;
		}

		public bool DropdownInteractable = false;
		[SerializeField] private int _dropdownIndex = -1;
		public int DropdownIndex {
			get {
				return _dropdownIndex;
			}
		}
		public IntCommand DropdownCommand;
		public bool DropdownCommand_CanExecute (object parameter) {
			return DropdownInteractable;
		}
		public void DropdownCommand_Execute (int value, object parameter) {
			_dropdownIndex = value;
		}

		public bool InputInteractable = false;
		[SerializeField] private string _inputText = "null";
		public string InputText {
			get {
				return _inputText;
			}
		}
		public StringCommand InputCommand;
		public bool InputCommand_CanExecute (object parameter) {
			return InputInteractable;
		}
		public void InputCommand_Execute (string value, object parameter) {
			_inputText = value;
		}

		protected override void BeforeAwake () {
			BtnCommand = new ButtonCommand (BtnCommand_CanExecute, BtnCommand_Execute);
			TglCommand = new BoolCommand (TglCommand_CanExecute, TglCommand_Execute);
			SliderCommand = new FloatCommand (SliderCommand_CanExecute, SliderCommand_Execute);
			DropdownCommand = new IntCommand (DropdownCommand_CanExecute, DropdownCommand_Execute);
			InputCommand = new StringCommand (InputCommand_CanExecute, InputCommand_Execute);
		}
	}
}