using UnityEngine;

namespace BaseAssets
{
    [CreateAssetMenu(fileName = "SettingsData", menuName = "BaseAssets/SettingsData")]
    public class Settings : SingletonScriptableObject<Settings>
    {
        public bool debugs;
        public bool haptics;
        public bool sounds;

        public float fadeTime;
    }
}
