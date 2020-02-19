using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Command;
using VVMUI.Core.Data;

public class TestSelectable : VMBehaviour {
	public ListData<BoolData> Tabs = new ListData<BoolData> () {
		true,
		false,
		false,
		false
	};

	private int selectTabIndex = 0;
	public ButtonCommand btnTest;
	public bool btnTest_CanExecute (object parameter) {
		return true;
	}
	public void btnText_Execute (object parameter) {
		selectTabIndex = (selectTabIndex + 1) % 4;
		Tabs[selectTabIndex].Set (true);
	}

	protected override void BeforeAwake () {
		btnTest = new ButtonCommand (btnTest_CanExecute, btnText_Execute);
	}
}