using System;

namespace VVMUI.Core.Converter {
    public class StringConverter : IConverter {
        public object Convert (object target, Type targetType, object parameter, VMBehaviour context) {
            return System.Convert.ToString (target);
        }

        public object ConvertBack (object target, Type targetType, object parameter, VMBehaviour context) {
            return target;
        }
    }
}