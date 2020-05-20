using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VVMUI.Core;
using VVMUI.Core.Command;
using VVMUI.Core.Data;

public class TestListOptVM : VMBehaviour {
	public class ItemData : StructData {
		public ColorData color = new ColorData (Color.white);
		public StringData name = new StringData ("");
	}

	public int Count;

	public ListData<ItemData> items = new ListData<ItemData> ();

	private List<Color> colors = new List<Color> () {
		Color.white,
			Color.red,
			Color.magenta,
			Color.cyan,
			Color.blue,
			Color.black
	};

	public ButtonCommand BtnFocusFirst;
	public bool BtnFocusFirst_CanExecute (object param) {
		return true;
	}
	public void BtnFocusFirst_Execute (object param) {
		items.FocusIndex = 0;
	}

	public ButtonCommand BtnFocusMiddle;
	public bool BtnFocusMiddle_CanExecute (object param) {
		return true;
	}
	public void BtnFocusMiddle_Execute (object param) {
		items.FocusIndex = items.Count / 2;
	}

	public ButtonCommand BtnFocusLast;
	public bool BtnFocusLast_CanExecute (object param) {
		return true;
	}
	public void BtnFocusLast_Execute (object param) {
		items.FocusIndex = items.Count - 1;
	}

	protected override void BeforeAwake(){
		BtnFocusFirst = new ButtonCommand(BtnFocusFirst_CanExecute, BtnFocusFirst_Execute);
		BtnFocusMiddle = new ButtonCommand(BtnFocusMiddle_CanExecute, BtnFocusMiddle_Execute);
		BtnFocusLast = new ButtonCommand(BtnFocusLast_CanExecute, BtnFocusLast_Execute);
	}

	protected override void BeforeActive () {
		items.Clear ();
		List<ItemData> list = new List<ItemData> ();
		for (int i = 0; i < Count; i++) {
			list.Add (new ItemData () {
				color = new ColorData (colors[i % colors.Count]),
					name = new StringData (i.ToString ())
			});
		}
		items.AddRange (list);
	}
}