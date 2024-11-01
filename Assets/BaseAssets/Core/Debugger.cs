using UnityEngine;

namespace BaseAssets
{
    public static class Debugger
    {
        public static void Log(string debug)
        {
            if (!Settings.Instance.debugs)
                return;
            Debug.Log(debug);
        }
    }
}
