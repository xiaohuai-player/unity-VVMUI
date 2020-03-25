using UnityEngine;

namespace VVMUI.Core.Data {
    [System.Serializable]
    public class ColorData : BaseData<Color> {
        public ColorData () {
        }

        public ColorData (Color v) {
            this.Set (v);
        }

        public static implicit operator ColorData (Color v) {
            return new ColorData (v);
        }

        public static implicit operator Color (ColorData d) {
			return d.Get ();
		}
    }
}