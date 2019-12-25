using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VVMUI.Core;
using VVMUI.Core.Command;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

public class TestComplexVM : VMBehaviour {
    public class FriendData : StructData {
        public StringData Name;
        public IntData Age;
        public BoolData IsOnline;
    }

    public ListData<FriendData> friends = new ListData<FriendData> () {
        new FriendData () {
        Name = "李明",
        Age = 14,
        IsOnline = true
        },
        new FriendData () {
        Name = "韩梅梅",
        Age = 15,
        IsOnline = true
        }
    };

    public StringData strInputName = "";
    public IntData intInputAge = 0;

    public StringCommand iptNameChanged;
    public bool iptNameChanged_CanExecute (object parameter) {
        return true;
    }
    public void iptNameChanged_Execute (string input, object parameter) {
        strInputName.Set (input);
    }

    public StringCommand iptAgeChanged;
    public bool iptAgeChanged_CanExecute (object parameter) {
        return true;
    }
    public void iptAgeChanged_Execute (string input, object parameter) {
        intInputAge.Set (Int32.Parse (input));
    }

    public ButtonCommand btnAddFriend;
    public bool btnAddFriend_CanExecute (object parameter) {
        return true;
    }
    public void btnAddFriend_Execute (object parameter) {
        friends.Add (new FriendData () {
            Name = strInputName.Get (),
                Age = intInputAge.Get (),
                IsOnline = true
        });
    }

    public ButtonCommand btnDelFriend;
    public bool btnDelFriend_CanExecute (object parameter) {
        return true;
    }
    public void btnDelFriend_Execute (object parameter) {
        friends.RemoveAt ((int) parameter);
    }

    protected override void BeforeAwake () {
        btnAddFriend = new ButtonCommand (btnAddFriend_CanExecute, btnAddFriend_Execute);
        btnDelFriend = new ButtonCommand (btnDelFriend_CanExecute, btnDelFriend_Execute);
        iptNameChanged = new StringCommand (iptNameChanged_CanExecute, iptNameChanged_Execute);
        iptAgeChanged = new StringCommand (iptAgeChanged_CanExecute, iptAgeChanged_Execute);
    }
}