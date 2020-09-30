using UnityEngine;

namespace VVMUI.Core
{
    public class Debugger
    {
        public static void Log(string tag, string msg)
        {
            Debug.Log(string.Format("VVMUI DEBUG [{0}]/I: {1}", tag, msg));
        }

        public static void LogError(string tag, string msg)
        {
            Debug.LogWarning(string.Format("VVMUI DEBUG [{0}]/E: {1}", tag, msg));
        }

        public static void LogWarning(string tag, string msg)
        {
            Debug.LogWarning(string.Format("VVMUI DEBUG [{0}]/W: {1}", tag, msg));
        }
    }
}