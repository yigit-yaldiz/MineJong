using System.Collections.Generic;
using UnityEngine;

namespace IndicatorAsset
{
    public class Indicator : MonoBehaviour
    {
        public static Indicator Instance { get; private set; }

        [SerializeField] private IndicatorText _prefab;

        private List<IndicatorText> _pool = new List<IndicatorText>();

        private List<IndicatorText> _usings = new List<IndicatorText>();

        private const int _poolCount = 10;
        private GameObject _poolParent;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _poolParent = new GameObject("Pool");
            _poolParent.transform.parent = transform;

            for (int i = 0; i < _poolCount; i++)
            {
                InstantiateNew();
            }
        }

        private void InstantiateNew()
        {
            IndicatorText newIT = Instantiate(_prefab, _poolParent.transform);
            newIT.Init();
            _pool.Add(newIT);
        }

        private IndicatorText GetIndicatorText()
        {
            if (_pool.Count <= 0)
                InstantiateNew();

            IndicatorText poolIT = _pool[0];
            _usings.Add(poolIT);
            _pool.RemoveAt(0);
            return poolIT;
        }
        internal void PutIndicatorText(IndicatorText indicatorText)
        {
            _usings.Remove(indicatorText);
            _pool.Add(indicatorText);
        }

        public void Show(Vector3 position, bool convertToScreen, string text, float duration, Color textColor, int textSize = 32, IndicatorAnimationTypes animationType = IndicatorAnimationTypes.Popup)
        {
            IndicatorText currentIT = GetIndicatorText();

            if (convertToScreen)
                position = Camera.main.WorldToScreenPoint(position);

            currentIT.SetPosition(position);
            currentIT.SetText(text);
            currentIT.SetColor(textColor);
            currentIT.SetSize(textSize);
            currentIT.Play(duration, animationType);
        }
    }
}
