using System;

namespace VVMUI.Core.Converter {
    public interface IConverter {
        object Convert (object value, Type targetType, object parameter, VMBehaviour context);
    }
}