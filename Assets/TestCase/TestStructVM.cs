using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Command;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

public class TestStructVM : VMBehaviour {
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

    public FriendData testFriend = new FriendData () {
        Name = "小明",
        Age = 15,
        IsOnline = false
    };

    public ButtonCommand btnAddAge;
    public bool btnAddAge_CanExecute (object parameter) {
        return true;
    }
    public void btnAddAge_Execute (object parameter) {
        testFriend.Age.Set (testFriend.Age.Get () + 1);
    }

    public IntToStringConverter cvtIntToString = new IntToStringConverter ();

    protected override void BeforeAwake () {
        btnAddAge = new ButtonCommand (btnAddAge_CanExecute, btnAddAge_Execute);
    }
}