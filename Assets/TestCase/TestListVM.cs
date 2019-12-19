using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Command;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

public class TestListVM : VMBehaviour {
    public ListData<StringData> lstTest = new ListData<StringData> () {
        "aaa",
        "bbb",
        "ccc"
    };

    public ButtonCommand btnAdd;
    private bool btnAdd_CanExecute () {
        return true;
    }
    private void btnAdd_Execute () {
        this.lstTest.Add ("add");
    }

    public ButtonCommand btnAddRange;
    private bool btnAddRange_CanExecute () {
        return true;
    }
    private void btnAddRange_Execute () {
        this.lstTest.AddRange (new List<StringData> () { "add range 1", "add range 2", "add range 3" });
    }

    public ButtonCommand btnRemove;
    private bool btnRemove_CanExecute () {
        return true;
    }
    private void btnRemove_Execute () {
        this.lstTest.RemoveAt (UnityEngine.Random.Range (0, this.lstTest.Count));
    }

    public ButtonCommand btnClear;
    private bool btnClear_CanExecute () {
        return true;
    }
    private void btnClear_Execute () {
        this.lstTest.Clear ();
    }

    public ButtonCommand btnInsert;
    private bool btnInsert_CanExecute () {
        return true;
    }
    private void btnInsert_Execute () {
        this.lstTest.Insert (UnityEngine.Random.Range (0, this.lstTest.Count), new StringData ("insert"));
    }

    public ButtonCommand btnInsertRange;
    private bool btnInsertRange_CanExecute () {
        return true;
    }
    private void btnInsertRange_Execute () {
        this.lstTest.InsertRange (UnityEngine.Random.Range (0, this.lstTest.Count), new List<StringData> () { "insert range 1", "isnert range 2", "insert range 3" });
    }

    public ButtonCommand btnReverse;
    private bool btnReverse_CanExecute () {
        return true;
    }
    private void btnReverse_Execute () {
        this.lstTest.Reverse ();
    }

    public ButtonCommand btnUpdate;
    private bool btnUpdate_CanExecute () {
        return true;
    }
    private void btnUpdate_Execute () {
        this.lstTest[UnityEngine.Random.Range (0, this.lstTest.Count)].Set ("update");
    }

    public ButtonCommand btnSet;
    private bool btnSet_CanExecute () {
        return true;
    }
    private void btnSet_Execute () {
        this.lstTest[UnityEngine.Random.Range (0, this.lstTest.Count)] = "set";
    }

    protected override void BeforeAwake () {
        btnAdd = new ButtonCommand (btnAdd_CanExecute, btnAdd_Execute);
        btnAddRange = new ButtonCommand (btnAddRange_CanExecute, btnAddRange_Execute);
        btnClear = new ButtonCommand (btnClear_CanExecute, btnClear_Execute);
        btnInsert = new ButtonCommand (btnInsert_CanExecute, btnInsert_Execute);
        btnInsertRange = new ButtonCommand (btnInsertRange_CanExecute, btnInsertRange_Execute);
        btnRemove = new ButtonCommand (btnRemove_CanExecute, btnRemove_Execute);
        btnReverse = new ButtonCommand (btnReverse_CanExecute, btnReverse_Execute);
        btnSet = new ButtonCommand (btnSet_CanExecute, btnSet_Execute);
        btnUpdate = new ButtonCommand (btnUpdate_CanExecute, btnUpdate_Execute);
    }
}