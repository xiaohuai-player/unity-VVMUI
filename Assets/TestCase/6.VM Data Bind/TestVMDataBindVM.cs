/**
 * @Author: #AUTHOR#
 * @Date: #CREATIONDATE#
 * @Description: this is description of TestVMDataBindVM
 */

using VVMUI.Core;
using VVMUI.Core.Data;

public class TestVMDataBindVM : VMBehaviour
{
    public class TestStructData : StructData
    {
        public StringData astring = new StringData("a of parent");
        public StringData bstring = new StringData("b of parent");
    }

    public TestStructData ParentStruct = new TestStructData();
}