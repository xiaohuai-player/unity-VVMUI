using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VVMUI.Core;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;
using UnityEditor;

namespace VVMUI.Core.Test
{
    public class BaseDataBindingTestVMBehaviour : VMBehaviour
    {
        public IntData testInt = new IntData(0);
        public BoolData testBool = new BoolData(false);
        public FloatData testFloat = new FloatData(0);
        public StringData testString = new StringData("");
        public ColorData testColor = new ColorData(new Color(0, 0, 0, 0));
        public TextureData testTexture = new TextureData();
        public ListData<BoolData> testToggles = new ListData<BoolData>() {
            new BoolData (true),
                new BoolData (true),
                new BoolData (false)
        };

        public StringConverter testConverter = new StringConverter();

        public BoolData testActive = new BoolData(false);

        public StringData testAnimation = new StringData("");

        public StringData testAnimator = new StringData("");

        protected override void BeforeActive()
        {
            base.BeforeActive();
            this.testTexture.Set(AssetDatabase.LoadAssetAtPath<Texture>("Assets/VVMUI/Tests/BaseDataBinding/Drawing_02.png"));
        }
    }
}