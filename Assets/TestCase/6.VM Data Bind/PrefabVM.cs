/**
 * @Author: #AUTHOR#
 * @Date: #CREATIONDATE#
 * @Description: this is description of PrefabVM
 */

using VVMUI.Core;
using VVMUI.Core.Data;

public class PrefabVM : VMBehaviour
{
    public StringData astring = new StringData("a");
    public StringData bstring = new StringData("b");
    public TestStructData strct = new TestStructData() {
        astring = new StringData("a of prefab"),
        bstring = new StringData("b of prefab"),
    };

    public class TestStructData : StructData
    {
        public StringData astring = new StringData();
        public StringData bstring = new StringData();
    }
}