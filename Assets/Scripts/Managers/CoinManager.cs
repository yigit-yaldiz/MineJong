using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniqueGames.Saving;

namespace Managers
{
    [RequireComponent(typeof(SaveableEntity))]
    public class CoinManager : MonoBehaviour, ISaveable
    {
        public static CoinManager Instance { get; private set; }
        public int Coin => _coin;

        int _coin;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            UIManager.Instance.PrintCoin();
        }

        public void IncreaseCoin(int increaseRate)
        {
            _coin += increaseRate;
            UIManager.Instance.PrintCoin();
        }

        public void DecreaseCoin(int rate)
        {
            if (_coin >= rate)
            {
                _coin -= rate;
            }
            else
            {
                Debug.LogWarning("There is not enough coin");
            }

            UIManager.Instance.PrintCoin();
        }

        [System.Serializable]
        struct SaveData
        {
            public int Coin;
        }

        public object CaptureState()
        {
            SaveData saveData = new();
            saveData.Coin = _coin;

            return saveData;
        }

        public void RestoreState(object state)
        {
            SaveData saveData = (SaveData)state;
            _coin = saveData.Coin;
        }
    }
}
