using System;
using System.Collections;
using System.Collections.Generic;

namespace VVMUI.Core.Data {
    public abstract class StructData : IData {
        public StructData () {
            InitFields ();
        }

        public StructData (Dictionary<string, IData> fields) {
            InitFields ();
        }

        private void InitFields () {

        }

        public event Action ValueChanged;
        public void InvokeValueChanged () {
            if (ValueChanged != null) {
                ValueChanged.Invoke ();
            }
        }

        public IData this [string key] {
            get {
                return null;
            }
        }
    }
}