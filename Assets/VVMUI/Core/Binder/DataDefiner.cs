using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder {
    public struct DefinerKey {
        public string Key;
        public int Index;
        public bool IsIndex;
    }

    [Serializable]
    public class DataDefiner {
        public string Key;
        public string Converter;
        public string ConverterParameter;

        private List<DefinerKey> keyChain = new List<DefinerKey> ();
        private bool keyParsed = false;
        public void ParseKey () {
            if (!keyParsed) {
                keyChain.Clear ();
                if (!string.IsNullOrEmpty (this.Key)) {
                    StringBuilder sb = new StringBuilder (10);
                    bool parseIndex = false;
                    for (int i = 0; i < this.Key.Length; i++) {
                        char c = this.Key[i];
                        if (c.Equals ('.')) {
                            DefinerKey k = new DefinerKey ();
                            k.Key = sb.ToString ();
                            keyChain.Add (k);
                            sb.Remove (0, sb.Length);
                        } else if (c.Equals ('[')) {
                            DefinerKey k = new DefinerKey ();
                            k.Key = sb.ToString ();
                            keyChain.Add (k);
                            sb.Remove (0, sb.Length);
                            parseIndex = true;
                        } else if (c.Equals (']') && parseIndex) {
                            int index = 0;
                            if (Int32.TryParse (sb.ToString (), out index)) {
                                DefinerKey k = new DefinerKey ();
                                k.Index = index;
                                k.IsIndex = true;
                                keyChain.Add (k);
                            }
                            sb.Remove (0, sb.Length);
                            parseIndex = false;
                        } else {
                            sb.Append (c);
                        }
                    }
                    if (sb.Length > 0 && !parseIndex) {
                        DefinerKey k = new DefinerKey ();
                        k.Key = sb.ToString ();
                        keyChain.Add (k);
                        sb.Remove (0, sb.Length);
                    }
                }
                keyParsed = true;
            }
        }

        public IData GetData (VMBehaviour vm) {
            ParseKey ();

            if (vm == null) {
                return null;
            }

            if (keyChain.Count <= 0) {
                return null;
            }

            if (keyChain[0].IsIndex) {
                return null;
            }

            IData data = vm.GetData (keyChain[0].Key);
            if (data != null) {
                for (int i = 1; i < keyChain.Count; i++) {
                    if (data == null) {
                        break;
                    }
                    DefinerKey dk = keyChain[i];
                    if (dk.IsIndex) {
                        IList ld = (data as IList);
                        if (ld != null && dk.Index >= 0 && dk.Index < ld.Count) {
                            data = (IData) ld[dk.Index];
                        }
                    } else {
                        StructData sd = data as StructData;
                        if (sd != null) {
                            data = sd[dk.Key];
                        }
                    }
                }
            }

            return data;
        }

        public IData GetData (IData source) {
            ParseKey ();

            if (source == null) {
                return null;
            }

            if (keyChain.Count <= 0) {
                return source;
            }

            if (keyChain[0].IsIndex) {
                return null;
            }

            StructData structSource = source as StructData;
            if (structSource == null) {
                return null;
            }

            IData data = structSource[keyChain[0].Key];
            if (data != null) {
                for (int i = 1; i < keyChain.Count; i++) {
                    if (data == null) {
                        break;
                    }
                    DefinerKey dk = keyChain[i];
                    if (dk.IsIndex) {
                        IList ld = data as IList;
                        if (ld != null && dk.Index >= 0 && dk.Index < ld.Count) {
                            data = (IData) ld[dk.Index];
                        }
                    } else {
                        StructData sd = data as StructData;
                        if (sd != null) {
                            data = sd[dk.Key];
                        }
                    }
                }
            }

            return data;
        }

        public IConverter GetConverter (VMBehaviour vm) {
            if (vm == null) {
                return null;
            }

            if (!string.IsNullOrEmpty (this.Converter)) {
                IConverter converter = vm.GetConverter (this.Converter);
                if (converter == null) {
                    Type converterType = Type.GetType ("VVMUI.Core.Converter." + this.Converter);
                    if (converterType != null) {
                        converter = (IConverter) Activator.CreateInstance (converterType);
                    }
                }
                return converter;
            }

            return null;
        }
    }
}