/**
 * @Author: #AUTHOR#
 * @Date: #CREATIONDATE#
 * @Description: this is description of TestBaseCommandVM
 */

using VVMUI.Core;
using VVMUI.Core.Command;

public class TestBaseCommandVM : VMBehaviour
{
    public VoidCommand btnCmd;
    public bool btnCmd_CanExecute(object param)
    {
        return true;
    }
    public void btnCmd_Execute(object param)
    {
        sliderCmdEnable = true;
    }

    private bool sliderCmdEnable = false;
    public FloatCommand sliderCmd;
    public bool sliderCmd_CanExecute(object param)
    {
        return sliderCmdEnable;
    }
    public void sliderCmd_Execute(float v, object param)
    {
        Debugger.Log("test", "slider:" + v);
    }

    protected override void BeforeAwake()
    {
        base.BeforeAwake();

        btnCmd = new VoidCommand(btnCmd_CanExecute, btnCmd_Execute);
        sliderCmd = new FloatCommand(sliderCmd_CanExecute, sliderCmd_Execute);
    }
}