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
    public class IntToStringConverter : IConverter {
        public object Convert (object value, Type targetType, object parameter, VMBehaviour context) {
            return System.Convert.ToString (value);
        }

        public object ConvertBack (object value, Type targetType, object parameter, VMBehaviour context) {
            return System.Convert.ToInt32 (value);
        }
    }

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

    public StringData strInputName = "测试输入";
    public IntData intInputAge = 99;
    public IntToStringConverter cvtTest = new IntToStringConverter ();

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
        strInputName.Set ("");
        intInputAge.Set (0);
    }

    public ButtonCommand btnDelFriend;
    public bool btnDelFriend_CanExecute (object index) {
        return (int) index != 1;
    }
    public void btnDelFriend_Execute (object index) {
        friends.RemoveAt ((int) index);
    }

    protected override void BeforeAwake () {
        btnAddFriend = new ButtonCommand (btnAddFriend_CanExecute, btnAddFriend_Execute);
        btnDelFriend = new ButtonCommand (btnDelFriend_CanExecute, btnDelFriend_Execute);
    }
}