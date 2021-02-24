/**
 * @Author: #AUTHOR#
 * @Date: #CREATIONDATE#
 * @Description: this is description of ExComponent
 */

using UnityEngine;

namespace VVMUI.Core
{
    public static class ExComponent
    {
        public static T GetComponentInParent<T>(this Component component, bool includeInactive = false)
        {
            T[] result = component.GetComponentsInParent<T>(includeInactive);
            if (result != null && result.Length > 0)
            {
                return result[0];
            }
            else
            {
                return default(T);
            }
        }
    }
}