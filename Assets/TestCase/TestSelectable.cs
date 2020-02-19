using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Data;

public class TestSelectable : VMBehaviour {
	public enum Tab {
		A,
		B,
		C,
		D
	}
	
	public ListData<BoolData> Tabs = new ListData<BoolData> () {
		false,
		false,
		false,
		false
	};
}