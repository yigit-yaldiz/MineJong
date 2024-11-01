using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Managers
{
    [DefaultExecutionOrder(1)]
    public class UILoader : MonoBehaviour
    {
        public static UILoader Instance { get; private set; }

        public static System.Action OnLevelFinish;

        [SerializeField] GameObject _failedPanel;
        [SerializeField] GameObject _completedPanel;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            GameManager.OnFinish += ActivateCompletedPanel;
        }

        private void OnDisable()
        {
            GameManager.OnFinish -= ActivateCompletedPanel;
        }

        public void ActiveFailedPanel()
        {
            _failedPanel.SetActive(true);
            OnLevelFinish?.Invoke();
        }

        public void ActivateCompletedPanel()
        {
            StartCoroutine(Delay());

            IEnumerator Delay()
            {
                yield return new WaitForSeconds(1f);
                _completedPanel.SetActive(true);
                OnLevelFinish?.Invoke();
            } 
        }
    }
}
