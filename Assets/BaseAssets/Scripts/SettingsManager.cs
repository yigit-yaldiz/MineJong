using System;
using UnityEngine;
using UnityEngine.UI;

namespace BaseAssets
{
    public class SettingsManager : MonoBehaviour
    {
        public static Action OnSettingsOpen;
        public static Action OnSettingsClose;

        [SerializeField] private GameObject _settingsPanel;

        [SerializeField] private Image _hapticImage;
        [SerializeField] private Image _soundImage;

        [SerializeField] private Sprite _hapticOn;
        [SerializeField] private Sprite _hapticOff;

        [SerializeField] private Sprite _soundOn;
        [SerializeField] private Sprite _soundOff;

        private void Start()
        {
            _hapticImage.sprite = Settings.Instance.haptics ? _hapticOn : _hapticOff;
            _soundImage.sprite = Settings.Instance.haptics ? _soundOn : _soundOff;
        }

        public void OpenSettings()
        {
            _settingsPanel.SetActive(true);

            OnSettingsOpen?.Invoke();
        }
        public void CloseSettings()
        {
            _settingsPanel.SetActive(false);

            OnSettingsClose?.Invoke();
        }

        public void HapticButton()
        {
            Settings.Instance.haptics = !Settings.Instance.haptics;
            _hapticImage.sprite = Settings.Instance.haptics ? _hapticOn : _hapticOff;
        }
        public void SoundButton()
        {
            Settings.Instance.sounds = !Settings.Instance.sounds;
            _soundImage.sprite = Settings.Instance.sounds ? _soundOn : _soundOff;
        }
    }
}