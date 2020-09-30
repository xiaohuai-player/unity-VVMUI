using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VVMUI.Core.Converter;
using VVMUI.Core.Data;

namespace VVMUI.Core.Binder
{
    public struct DefinerKey
    {
        public string Key;
        public int Index;
        public string HashKey;

        public DefinerKey(string key = null, int index = -1, string hashKey = null)
        {
            this.Key = key;
            this.Index = index;
            this.HashKey = hashKey;
        }

        public IData GetData(IData data)
        {
            IData result = null;

            if (Index > -1)
            {
                IList ld = data as IList;
                if (ld != null && Index >= 0 && Index < ld.Count)
                {
                    result = (IData)ld[Index];
                }
            }

            if (!string.IsNullOrEmpty(HashKey))
            {
                IDictionary dict = data as IDictionary;
                if (dict != null && dict.Contains(HashKey))
                {
                    result = (IData)dict[HashKey];
                }
            }

            if (result == null)
            {
                StructData structdata = data as StructData;
                if (structdata != null)
                {
                    result = structdata[Key];
                }
            }

            return result;
        }

        public IData GetData(VMBehaviour vm)
        {
            // 从 VMBehaviour 中获取数据只能是属性
            if (Index > -1 || !string.IsNullOrEmpty(HashKey))
            {
                return null;
            }
            return vm.GetData(Key);
        }
    }

    [Serializable]
    public class DataDefiner
    {

        // KeyChain 示例：data.friends[5].characters["adikia"].name
        public string Key;
        public string Converter;
        public string ConverterParameter;

        private List<DefinerKey> keyChain = new List<DefinerKey>();
        private bool keyParsed = false;
        public void ParseKey()
        {
            if (!keyParsed)
            {
                keyChain.Clear();
                if (!string.IsNullOrEmpty(this.Key))
                {
                    StringBuilder sb = new StringBuilder(10);
                    bool parseIndex = false;
                    bool parseHash = false;
                    for (int i = 0; i < this.Key.Length; i++)
                    {
                        char c = this.Key[i];
                        if (c.Equals('.'))
                        {
                            string key = sb.ToString();
                            if (!string.IsNullOrEmpty(key))
                            {
                                DefinerKey k = new DefinerKey(key);
                                keyChain.Add(k);
                            }
                            sb.Remove(0, sb.Length);
                        }
                        else if (c.Equals('['))
                        {
                            string key = sb.ToString();
                            if (!string.IsNullOrEmpty(key))
                            {
                                DefinerKey k = new DefinerKey(key);
                                keyChain.Add(k);
                            }
                            sb.Remove(0, sb.Length);
                            parseIndex = true;
                        }
                        else if (c.Equals('"') && parseIndex && !parseHash)
                        {
                            sb.Remove(0, sb.Length);
                            parseIndex = false;
                            parseHash = true;
                        }
                        else if (c.Equals('"') && !parseIndex && parseHash)
                        {
                            string key = sb.ToString();
                            if (!string.IsNullOrEmpty(key))
                            {
                                DefinerKey k = new DefinerKey(null, -1, key);
                                keyChain.Add(k);
                            }
                            sb.Remove(0, sb.Length);
                            parseIndex = true;
                            parseHash = false;
                        }
                        else if (c.Equals(']') && parseIndex)
                        {
                            int index = 0;
                            string key = sb.ToString();
                            if (Int32.TryParse(key, out index))
                            {
                                DefinerKey k = new DefinerKey(null, index, null);
                                keyChain.Add(k);
                            }
                            sb.Remove(0, sb.Length);
                            parseIndex = false;
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    if (sb.Length > 0 && !parseIndex)
                    {
                        DefinerKey k = new DefinerKey(sb.ToString());
                        keyChain.Add(k);
                        sb.Remove(0, sb.Length);
                    }
                }
                keyParsed = true;
            }
        }

        public IData GetData(VMBehaviour vm)
        {
            ParseKey();

            if (vm == null)
            {
                return null;
            }

            if (keyChain.Count <= 0)
            {
                return null;
            }

            // 从 VMBehaviour 中获取数据第一项只能是属性
            if (keyChain[0].Index > -1 || !string.IsNullOrEmpty(keyChain[0].HashKey))
            {
                return null;
            }

            IData data = keyChain[0].GetData(vm);
            for (int i = 1; i < keyChain.Count; i++)
            {
                if (data == null)
                {
                    break;
                }
                data = keyChain[i].GetData(data);
            }

            return data;
        }

        public IData GetData(IData source)
        {
            ParseKey();

            if (source == null)
            {
                return null;
            }

            if (keyChain.Count <= 0)
            {
                return source;
            }

            IData data = source;
            for (int i = 0; i < keyChain.Count; i++)
            {
                if (data == null)
                {
                    break;
                }
                data = keyChain[i].GetData(data);
            }

            return data;
        }

        public IConverter GetConverter(VMBehaviour vm)
        {
            if (vm == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(this.Converter))
            {
                IConverter converter = vm.GetConverter(this.Converter);
                if (converter == null)
                {
                    Type converterType = Type.GetType("VVMUI.Core.Converter." + this.Converter);
                    if (converterType != null)
                    {
                        converter = (IConverter)Activator.CreateInstance(converterType);
                    }
                }
                return converter;
            }

            return null;
        }
    }
}