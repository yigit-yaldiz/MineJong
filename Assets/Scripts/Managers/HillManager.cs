using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class HillManager : MonoBehaviour
    {
        public static HillManager Instance { get; private set; }
        public Hill ActiveHill { get => _activeHill; set => _activeHill = value; }
        public int ActiveHillBlockCount { get => _activeHillBlockCount; set => _activeHillBlockCount = value; }

        [SerializeField] Hill _activeHill;

        int _activeHillBlockCount;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            GameManager.OnFinish += DisableHill;
        }

        private void OnDisable()
        {
            GameManager.OnFinish -= DisableHill;
        }

        private void Start()
        {
            FindCurrentHill();
        }

        void FindCurrentHill()
        {
            _activeHill = GetComponentInChildren<Hill>();
            _activeHillBlockCount = _activeHill.BlockCount;
        }

        void DisableHill()
        {
            _activeHill.IsFinished = true;
            _activeHill.gameObject.SetActive(false);
            GameManager.Instance.ResetReadyBlockCount();
        }
    }
}
