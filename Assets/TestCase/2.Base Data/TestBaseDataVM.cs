/**
 * @Author: #AUTHOR#
 * @Date: #CREATIONDATE#
 * @Description: this is description of TestBaseDataVM
 */

using VVMUI.Core;
using VVMUI.Core.Data;

public class TestBaseDataVM : VMBehaviour
{
    public class TestItemStructData : StructData
    {
        public DictionaryData<StringData> dict = new DictionaryData<StringData>() { { "key", new StringData("value") } };
    }

    public class TestStructData : StructData
    {
        public ListData<TestItemStructData> field = new ListData<TestItemStructData>() { new TestItemStructData() };
    }

    public StringData testString = new StringData("test");
    public BoolData testBool = new BoolData(false);
    public StringData testAnimation = new StringData("anim1");
    public StringData testAnimator = new StringData("state1");
    public TestStructData strct = new TestStructData();
}