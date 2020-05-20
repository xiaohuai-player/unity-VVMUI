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

    public class Body {
        public int Height;
        public int Weight;
    }

    public class Friend {
        public string Name;
        public int Age;
        public bool IsOnline;
        public Body Body;
        public List<int> Scores;
    }

    public class BodyData : StructData {
        public IntData Height = new IntData ();
        public IntData Weight = new IntData ();
    }

    public class FriendData : StructData {
        public StringData Name = new StringData ();
        public IntData Age = new IntData ();
        public BoolData IsOnline = new BoolData ();
        public BodyData Body = new BodyData ();
        public ListData<IntData> Scores;
    }

    public ListData<FriendData> friends = new ListData<FriendData> ();

    public StringData strInputName;
    public IntData intInputAge;
    public SpriteData spriteTest;
    public IntToStringConverter cvtTest = new IntToStringConverter ();

    public ButtonCommand btnAddFriend;
    public bool btnAddFriend_CanExecute (object parameter) {
        return true;
    }
    public void btnAddFriend_Execute (object parameter) {
        friends.Add (new FriendData () {
            Name = new StringData (strInputName.Get ()),
            Age = new IntData (intInputAge.Get ()),
            IsOnline = new BoolData (true)
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

    public FriendData testFriend = new FriendData ();

    public DictionaryData<FriendData> testDictFriend;

    protected override void BeforeAwake () {
        btnAddFriend = new ButtonCommand (btnAddFriend_CanExecute, btnAddFriend_Execute);
        btnDelFriend = new ButtonCommand (btnDelFriend_CanExecute, btnDelFriend_Execute);

        Friend f = new Friend () {
            Name = "testabc",
            Age = 12,
            IsOnline = false,
            Body = new Body () {
                Height = 170,
                Weight = 75
            },
            Scores = new List<int> {
                100,
                95,
                90
            }
        };
        testFriend.Parse (f);

        List<Friend> fs = new List<Friend>(){
            new Friend(){
                Name = "李明",
                Age = 15,
                IsOnline = false,
                Body = new Body () {
                    Height = 170,
                    Weight = 75
                },
                Scores = new List<int>(){
                    60, 75
                }
            },
            new Friend(){
                Name = "韩梅梅",
                Age = 12,
                IsOnline = false,
                Body = new Body () {
                    Height = 160,
                    Weight = 55
                },
                Scores = new List<int>(){
                    80, 15
                }
            }
        };
        friends.Parse(fs);

        Dictionary<string, Friend> dictFs = new Dictionary<string, Friend>(){
            { "lm",  fs[0]},
            { "hmm", fs[1]}
        };
        testDictFriend = DictionaryData.Parse<FriendData>(dictFs);
    }
}