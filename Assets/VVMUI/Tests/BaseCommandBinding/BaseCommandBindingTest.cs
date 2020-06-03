using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace VVMUI.Core.Test {
	public class BaseCommandBindingTest : IPrebuildSetup {
		public void Setup () {
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject> ("Assets/VVMUI/Core/Tests/BaseCommandBinding/BaseCommandBindingTestCanvas.prefab");
			GameObject obj = GameObject.Instantiate (prefab);
			obj.name = "BaseCommandBindingTestCanvas";
		}

		[UnityTest]
		public IEnumerator ButtonCommandPasses () {
			yield return null;

			GameObject obj = GameObject.Find ("BaseCommandBindingTestCanvas");
			BaseCommandBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseCommandBindingTestVMBehaviour> ();

			vm.BtnInteractable = false;
			vm.NotifyCommandsCanExecute ();
			Assert.That (vm.transform.Find ("Button").GetComponent<Button> ().interactable, Is.False, "ButtonCommand can_execute set to false failed.");

			vm.BtnInteractable = true;
			vm.NotifyCommandsCanExecute ();
			Assert.That (vm.transform.Find ("Button").GetComponent<Button> ().interactable, Is.True, "ButtonCommand can_execute set to true failed.");

			vm.BtnClicked = false;
			vm.transform.Find ("Button").GetComponent<Button> ().onClick.Invoke ();
			Assert.That (vm.BtnClicked, "ButtonCommand execute failed.");
		}

		[UnityTest]
		public IEnumerator ToggleCommandPasses () {
			yield return null;

			GameObject obj = GameObject.Find ("BaseCommandBindingTestCanvas");
			BaseCommandBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseCommandBindingTestVMBehaviour> ();

			vm.ToggleInteractable = false;
			vm.NotifyCommandsCanExecute ();
			Assert.That (vm.transform.Find ("Toggle").GetComponent<Toggle> ().interactable, Is.False, "ToggleCommand can_execute set to false failed.");

			vm.ToggleInteractable = true;
			vm.NotifyCommandsCanExecute ();
			Assert.That (vm.transform.Find ("Toggle").GetComponent<Toggle> ().interactable, Is.True, "ToggleCommand can_execute set to true failed.");

			vm.transform.Find ("Toggle").GetComponent<Toggle> ().isOn = true;
			vm.transform.Find ("Toggle").GetComponent<Toggle> ().isOn = false;
			Assert.That (vm.ToggleIsOn, Is.False, "ToggleCommand execute false failed.");

			vm.transform.Find ("Toggle").GetComponent<Toggle> ().isOn = true;
			Assert.That (vm.ToggleIsOn, Is.True, "ToggleCommand execute true failed.");
		}

		[UnityTest]
		public IEnumerator SliderCommandPasses () {
			yield return null;

			GameObject obj = GameObject.Find ("BaseCommandBindingTestCanvas");
			BaseCommandBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseCommandBindingTestVMBehaviour> ();

			vm.SliderInteractable = false;
			vm.NotifyCommandsCanExecute ();
			Assert.That (vm.transform.Find ("Slider").GetComponent<Slider> ().interactable, Is.False, "SliderCommand can_execute set to false failed.");

			vm.SliderInteractable = true;
			vm.NotifyCommandsCanExecute ();
			Assert.That (vm.transform.Find ("Slider").GetComponent<Slider> ().interactable, Is.True, "SliderCommand can_execute set to true failed.");

			vm.transform.Find ("Slider").GetComponent<Slider> ().value = 3.5f;
			Assert.That (Mathf.Abs (vm.SliderValue - vm.transform.Find ("Slider").GetComponent<Slider> ().value), Is.LessThan (0.001f), "SliderCommand execute 3.5f failed.");

			vm.transform.Find ("Slider").GetComponent<Slider> ().value = 0f;
			Assert.That (Mathf.Abs (vm.SliderValue - vm.transform.Find ("Slider").GetComponent<Slider> ().value), Is.LessThan (0.001f), "SliderCommand execute 0f failed.");
		}

		[UnityTest]
		public IEnumerator DropdownCommandPasses () {
			yield return null;

			GameObject obj = GameObject.Find ("BaseCommandBindingTestCanvas");
			BaseCommandBindingTestVMBehaviour vm = obj.GetComponentInChildren<BaseCommandBindingTestVMBehaviour> ();

			vm.DropdownInteractable = false;
			vm.NotifyCommandsCanExecute ();
			Assert.That (vm.transform.Find ("Dropdown").GetComponent<Dropdown> ().interactable, Is.False, "DropdownCommand can_execute set to false failed.");

			vm.DropdownInteractable = true;
			vm.NotifyCommandsCanExecute ();
			Assert.That (vm.transform.Find ("Dropdown").GetComponent<Dropdown> ().interactable, Is.True, "DropdownCommand can_execute set to true failed.");

			vm.transform.Find ("Dropdown").GetComponent<Dropdown> ().value = 1;
			Assert.That (vm.DropdownIndex, Is.EqualTo (1), "DropdownCommand execute 1 failed.");

			vm.transform.Find ("Dropdown").GetComponent<Dropdown> ().value = 2;
			Assert.That (vm.DropdownIndex, Is.EqualTo (2), "DropdownCommand execute 2 failed.");

			vm.transform.Find ("Dropdown").GetComponent<Dropdown> ().value = 0;
			Assert.That (vm.DropdownIndex, Is.EqualTo (0), "DropdownCommand execute 0 failed.");
		}
	}
}