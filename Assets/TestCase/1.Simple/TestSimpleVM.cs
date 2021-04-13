/**
 * @Author: #AUTHOR#
 * @Date: #CREATIONDATE#
 * @Description: this is description of TestSimpleVM
 */

using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Data;
using VVMUI.Core.Command;

public class TestSimpleVM : VMBehaviour
{
    public StringData testData = new StringData();

    public VoidCommand testCmd;
    public void testCmd_Execute(object parameter)
    {
        testData.Set("test cmd executed.");
    }

    protected override void BeforeAwake()
    {
        base.BeforeAwake();
        testCmd = new VoidCommand(null, testCmd_Execute);
    }

    protected override void BeforeActive()
    {
        base.BeforeActive();
        testData.Set("actived.");
    }
}