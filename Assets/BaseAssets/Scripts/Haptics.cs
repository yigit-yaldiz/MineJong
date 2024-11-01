namespace BaseAssets
{
    public partial class BA
    {
        public enum HapticTypes { Selection, Success, Warning, Failure, LightImpact, MediumImpact, HeavyImpact, None }
        public static void PlayHaptic(HapticTypes haptic)
        {
            if (!Settings.Instance.haptics)
                return;

            Vibration.Haptic(haptic);
        }
    }
}