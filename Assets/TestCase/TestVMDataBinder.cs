using System;
using System.Collections.Generic;
using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Command;
using VVMUI.Core.Data;
using VVMUI.Core.Converter;

public class TestVMDataBinder : VMBehaviour
{
    public class TestPrefabData : StructData
    {
        public StringData Name = new StringData("");
        public IntData Age = new IntData(0);
        public ListData<IntData> Scores = new ListData<IntData>();
    }

    public class TestListData : StructData
    {
        public IntData Index;
        public TestPrefabData PrefabData;
    }

    public class IntToStringConverter : IConverter
    {
        public object Convert(object value, Type targetType, object parameter, VMBehaviour context)
        {
            return System.Convert.ToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, VMBehaviour context)
        {
            return System.Convert.ToInt32(value);
        }
    }

    public IntToStringConverter cvtIntToString = new IntToStringConverter();

    public TestPrefabData TestSingle = new TestPrefabData();
    public ListData<TestListData> TestList = new ListData<TestListData>();

    public ButtonCommand btnChangeSingle;
    public void btnChangeSingle_Execute(object parameter)
    {
        TestSingle.Name.Set("name changed");
        TestSingle.Age.Set(TestSingle.Age.Get() + 1);
        TestSingle.Scores[0].Set(TestSingle.Scores[0].Get() + 1);
    }

    public ButtonCommand btnChangeListItem;
    public void btnChangeListItem_Execute(object parameter)
    {
        // TestList[Random.Range(0, TestList.Count)] = new TestPrefabData()
        // {
        //     Name = new StringData("name changed"),
        //     Age = new IntData(TestList[0].Age.Get() + 1),
        //     Scores = new ListData<IntData>()
        // };
        // TestList[1].Scores.RemoveAt(0);
        // TestList[1].Scores[0].Set(0);
    }

    protected override void BeforeAwake()
    {
        base.BeforeAwake();

        btnChangeSingle = new ButtonCommand(null, this.btnChangeSingle_Execute);
        btnChangeListItem = new ButtonCommand(null, this.btnChangeListItem_Execute);
    }

    protected override void BeforeActive()
    {
        base.BeforeActive();

        TestSingle.Name.Set("test single");
        TestSingle.Age.Set(11);
        for (int i = 0; i < 3; i++)
        {
            TestSingle.Scores.Add(new IntData(UnityEngine.Random.Range(60, 100)));
        }

        List<TestListData> tmp = new List<TestListData>();
        for (int i = 0; i < 5; i++)
        {
            TestPrefabData d = new TestPrefabData();
            d.Name.Set("item" + i);
            d.Age.Set(i);
            for (int j = 0; j < 3; j++)
            {
                d.Scores.Add(new IntData(UnityEngine.Random.Range(100, 120)));
            }

            TestListData ld = new TestListData()
            {
                Index = new IntData(i),
                PrefabData = d
            };
            tmp.Add(ld);
        }
        TestList.AddRange(tmp);
    }
}
