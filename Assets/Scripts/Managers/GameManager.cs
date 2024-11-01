using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniqueGames.Saving;
using Saving;

namespace Managers
{
    public enum GameState
    {
        Loading,
        Collect,
        Build,
        Furnace
    }

    [RequireComponent(typeof(SaveableEntity))]
    public class GameManager : MonoBehaviour, ISaveable
    {
        public static System.Action OnFinish;

        public static GameManager Instance { get; private set; }
        public GameState GameState { get => _gameState; set => _gameState = value; }

        [SerializeField] int _targetFPS;
        [SerializeField] GameState _gameState;

        int _readyBlockCount;

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Application.targetFrameRate = _targetFPS;

            if (_gameState != GameState.Collect) return;

            StartCoroutine(CheckCompleted());
            
            IEnumerator CheckCompleted()
            {
                yield return new WaitForSeconds(0.01f);

                if (_readyBlockCount == HillManager.Instance.ActiveHillBlockCount / 3)
                {
                    UILoader.Instance.ActivateCompletedPanel();
                }
            }
        }

        public void IncreaseReadyBlockCount()
        {
            _readyBlockCount++;

            if (_readyBlockCount == HillManager.Instance.ActiveHillBlockCount / 3)
            {
                OnFinish?.Invoke();
            }
        }

        public void ResetReadyBlockCount()
        {
            _readyBlockCount = 0;
        }

        [System.Serializable]
        struct SaveData
        {
            public int BlockCount;
            public int LevelIndex;
        }

        public object CaptureState()
        {
            SaveData saveData = new();
            saveData.BlockCount = _readyBlockCount;

            return saveData;
        }

        public void RestoreState(object state)
        {
            SaveData saveData = (SaveData)state;
            _readyBlockCount = saveData.BlockCount;
        }
    }
}
