using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace VVMUI.Core.Test
{
    public class BaseDataBindingTest : IPrebuildSetup
    {
        public void Setup()
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/VVMUI/Tests/BaseDataBinding/BaseDataBindingTestCanvas.prefab");
            GameObject obj = GameObject.Instantiate(prefab);
            obj.name = "BaseDataBindingTestCanvas";
        }

        [UnityTest]
        public IEnumerator BaseIntTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.testInt.Set(0);
            Assert.That(vm.transform.Find("int").GetComponent<Dropdown>().value == 0, "int set to 0 failed");

            vm.testInt.Set(2);
            Assert.That(vm.transform.Find("int").GetComponent<Dropdown>().value == 2, "int set to 2 failed");

            vm.testInt.Set(1);
            Assert.That(vm.transform.Find("int").GetComponent<Dropdown>().value == 1, "int set to 1 failed");
        }

        [UnityTest]
        public IEnumerator BaseBoolTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.testBool.Set(false);
            Assert.That(vm.transform.Find("bool").GetComponent<Toggle>().isOn == false, "bool set to false failed");

            vm.testBool.Set(true);
            Assert.That(vm.transform.Find("bool").GetComponent<Toggle>().isOn == true, "bool set to true failed");
        }

        [UnityTest]
        public IEnumerator BaseFloatTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.testFloat.Set(0.5f);
            Assert.That(Mathf.Abs(vm.transform.Find("float").GetComponent<Slider>().value - 0.5f) < 0.001f, "float set to 0.5f failed");

            vm.testFloat.Set(0f);
            Assert.That(Mathf.Abs(vm.transform.Find("float").GetComponent<Slider>().value - 0f) < 0.001f, "float set to 0f failed");

            vm.testFloat.Set(3.75f);
            Assert.That(Mathf.Abs(vm.transform.Find("float").GetComponent<Slider>().value - 3.75f) < 0.001f, "float set to 3.75f failed");
        }

        [UnityTest]
        public IEnumerator BaseStringTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.testString.Set("hello");
            Assert.That(vm.transform.Find("string").GetComponent<UnityEngine.UI.Text>().text.Equals("hello"), "string set to 'hello' failed");
        }

        [UnityTest]
        public IEnumerator BaseColorTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.testColor.Set(new Color(121, 232, 101, 55));
            Assert.That(delegate ()
            {
                Color imgColor = vm.transform.Find("color").GetComponent<Image>().color;
                return Mathf.Abs(imgColor.r - 121.0f) < 0.001f && Mathf.Abs(imgColor.g - 232.0f) < 0.001f && Mathf.Abs(imgColor.b - 101.0f) < 0.001f && Mathf.Abs(imgColor.a - 55.0f) < 0.001f;
            });
        }

        [UnityTest]
        public IEnumerator BaseTextureTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.testTexture.Set(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/VVMUI/Tests/BaseDataBinding/Drawing_02.png"));
            Assert.That(delegate ()
            {
                Texture2D tex = vm.transform.Find("texture").GetComponent<RawImage>().texture as Texture2D;
                return tex == AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/VVMUI/Tests/BaseDataBinding/Drawing_02.png");
            }, "tex1 set failed");

            vm.testTexture.Set(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/VVMUI/Tests/BaseDataBinding/gezi1.png"));
            Assert.That(delegate ()
            {
                Texture2D tex = vm.transform.Find("texture").GetComponent<RawImage>().texture as Texture2D;
                return tex == AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/VVMUI/Tests/BaseDataBinding/gezi1.png");
            }, "tex2 set failed");
        }

        [UnityTest]
        public IEnumerator BaseConverterTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.testInt.Set(2);
            Assert.That(vm.transform.Find("converter").GetComponent<UnityEngine.UI.Text>().text.Equals("2"), "int set to 2 failed");
        }

        [UnityTest]
        public IEnumerator ToggleDuplexTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.transform.Find("bool").GetComponent<Toggle>().isOn = true;
            Assert.That(vm.testBool.Get(), Is.True, "toggle set to true failed");

            vm.transform.Find("bool").GetComponent<Toggle>().isOn = false;
            Assert.That(vm.testBool.Get(), Is.False, "toggle set to false failed");
        }

        [UnityTest]
        public IEnumerator DropdownDuplexTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.transform.Find("int").GetComponent<Dropdown>().value = 0;
            Assert.That(vm.testInt.Get(), Is.EqualTo(0), "dropdown set to 0 failed");

            vm.transform.Find("int").GetComponent<Dropdown>().value = 1;
            Assert.That(vm.testInt.Get(), Is.EqualTo(1), "dropdown set to 1 failed");

            vm.transform.Find("int").GetComponent<Dropdown>().value = 2;
            Assert.That(vm.testInt.Get(), Is.EqualTo(2), "dropdown set to 2 failed");
        }

        [UnityTest]
        public IEnumerator SliderDuplexTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.transform.Find("float").GetComponent<Slider>().value = 2.4f;
            Assert.That(Mathf.Abs(vm.testFloat.Get() - 2.4f), Is.LessThanOrEqualTo(0.001f), "slider set to 2.4f failed");
        }

        [UnityTest]
        public IEnumerator InputDuplexTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.transform.Find("input").GetComponent<InputField>().text = "input test";
            Assert.That(vm.testString.Get(), Is.EqualTo("input test"), "input set to 'input test' failed");
        }

        [UnityTest]
        public IEnumerator ToggleGroupTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            ToggleGroup tg = vm.transform.Find("toggles").GetComponent<ToggleGroup>();
            Toggle toggle1 = tg.transform.GetChild(0).GetComponent<Toggle>();
            Toggle toggle2 = tg.transform.GetChild(1).GetComponent<Toggle>();
            Toggle toggle3 = tg.transform.GetChild(2).GetComponent<Toggle>();

            vm.testToggles[0].Set(true);
            Assert.That(toggle1.isOn && !toggle2.isOn && !toggle3.isOn, "vm toggle1 fail");

            vm.testToggles[1].Set(true);
            Assert.That(!toggle1.isOn && toggle2.isOn && !toggle3.isOn, "vm toggle2 fail");

            vm.testToggles[2].Set(true);
            Assert.That(!toggle1.isOn && !toggle2.isOn && toggle3.isOn, "vm toggle3 fail");

            toggle1.isOn = true;
            Assert.That(vm.testToggles[0].Get() && !vm.testToggles[1].Get() && !vm.testToggles[2].Get(), "toggle group toggle1 fail");

            toggle2.isOn = true;
            Assert.That(!vm.testToggles[0].Get() && vm.testToggles[1].Get() && !vm.testToggles[2].Get(), "toggle group toggle2 fail");

            toggle3.isOn = true;
            Assert.That(!vm.testToggles[0].Get() && !vm.testToggles[1].Get() && vm.testToggles[2].Get(), "toggle group toggle3 fail");
        }

        [UnityTest]
        public IEnumerator ActiveTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.testActive.Set(true);
            Assert.That(vm.transform.Find("active").gameObject.activeSelf, "active set to true failed");

            vm.testActive.Set(false);
            Assert.That(vm.transform.Find("active").gameObject.activeSelf, Is.False, "active set to false failed");
        }

        [UnityTest]
        public IEnumerator AnimationTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();

            vm.testAnimation.Set("animationTest1");
            yield return null;
            Assert.That(vm.transform.Find("animation").GetComponent<Animation>().IsPlaying("animationTest1"), "animationTest1 failed");

            vm.testAnimation.Set("animationTest2");
            yield return null;
            Assert.That(vm.transform.Find("animation").GetComponent<Animation>().IsPlaying("animationTest2"), "animationTest2 failed");
        }

        [UnityTest]
        public IEnumerator AnimatorTestPasses()
        {
            yield return null;

            GameObject obj = GameObject.Find("BaseDataBindingTestCanvas");
            BaseDataBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseDataBindingTestVMBehaviour>();
            Animator animator = vm.transform.Find("animator").GetComponent<Animator>();

            vm.testAnimator.Set("animatorTest1");
            yield return null;
            Assert.That(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name, Is.EqualTo("animatorTest1"), "animatorTest1 failed");

            vm.testAnimator.Set("animatorTest2");
            yield return null;
            Assert.That(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name, Is.EqualTo("animatorTest2"), "animatorTest2 failed");
        }
    }
}