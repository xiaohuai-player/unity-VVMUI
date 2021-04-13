/**
 * @Author: #AUTHOR#
 * @Date: #CREATIONDATE#
 * @Description: this is description of TestStructDataVM
 */

using VVMUI.Core;
using VVMUI.Core.Data;

public class TestStructDataVM : VMBehaviour
{
    public class TestInnerStructData : StructData
    {
        public StringData TestInnerString = new StringData("test inner string");
    }

    public class TestStructData : StructData
    {
        public StringData TestString = new StringData("test string");
        public BoolData TestBool = new BoolData(false);
        public TestInnerStructData TestInnerStruct = new TestInnerStructData();
    }

    public TestStructData testStrct = new TestStructData();
}