/**
 * @Author: #AUTHOR#
 * @Date: #CREATIONDATE#
 * @Description: this is description of TestListDataVM
 */

using System.Collections;
using System.Collections.Generic;
using VVMUI.Core;
using VVMUI.Core.Data;
using VVMUI.Core.Command;

public class TestListDataVM : VMBehaviour
{
    public class TestStructData : StructData
    {
        public StringData TestString = new StringData("test struct string");
    }

    public ListData<ListData<StringData>> innerList = new ListData<ListData<StringData>>() { 
        new ListData<StringData>(){ new StringData("inner list item 0") }
    };
    public ListData<StringData> testList = new ListData<StringData>() { new StringData("test item 0") };
    public ListData<TestStructData> testStrctList = new ListData<TestStructData>() { new TestStructData() };

    public VoidCommand btnCmd;
    public void btnCmd_Execute(object param)
    {
        List<StringData> data = new List<StringData>();
        for (int i = 0; i < 200; i++)
        {
            data.Add(new StringData(i.ToString()));
        }
        testList.Clear();
        testList.AddRange(data);
    }

    public VoidCommand itemBtnCmd;
    public void itemBtnCmd_Execute(object index)
    {
        Debugger.Log("test", ((int)index).ToString());
    }

    protected override void BeforeAwake()
    {
        base.BeforeAwake();
        btnCmd = new VoidCommand(null, btnCmd_Execute);
        itemBtnCmd = new VoidCommand(null, itemBtnCmd_Execute);
    }
}