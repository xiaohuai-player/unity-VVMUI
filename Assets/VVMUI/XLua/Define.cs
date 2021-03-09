namespace VVMUI.Script.XLua
{
    public enum XLuaDataType
    {
        // base types
        Boolean,
        Float,
        Int,
        String,
        Color,
        Vector2,
        Vector3,
        Rect,
        Sprite,
        Texture,

        // complex types
        Struct,
        List,
        Dictionary
    }

    public enum XLuaCommandType
    {
        Void,
        Bool,
        Float,
        Int,
        String,
        Vector2
    }

}