﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Command;
using VVMUI.Core.Data;

public class TestSelectable : VMBehaviour {
	public ListData<BoolData> Tabs = new ListData<BoolData> () {
		new BoolData(true),
		new BoolData(false),
		new BoolData(false),
		new BoolData(false)
	};

	public ButtonCommand btnTest2;
	public FloatCommand sliderTest;

	public FloatData sliderValue;

	protected override void BeforeAwake () {
		int selectTabIndex = 0;
		this.AddCommand ("btnTest", new ButtonCommand (
			delegate (object parameter) {
				return true;
			},
			delegate (object parameter) {
				selectTabIndex = (selectTabIndex + 1) % 4;
				Tabs[selectTabIndex].Set (true);
			}
		));
	}
}