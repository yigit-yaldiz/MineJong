using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniqueGames.Saving;
using Saving;

namespace Managers
{
    [RequireComponent(typeof(SaveableEntity))]
    public class LevelLoader : MonoBehaviour, ISaveable
    {
        public static LevelLoader Instance { get; private set; }
        public int ActiveSceneIndex => _activeSceneIndex;

        int _activeSceneIndex;

        Animator _transition;

        const int _loadingSceneIndex = 0;
        const int _buildSceneIndex = 1;
        const int _firstCollectSceneIndex = 2;
        const int _furnaceSceneIndex = 7; 

        int _lastCollectSceneIndex;

        private void Awake()
        {
            Instance = this;
            _transition = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            if (_activeSceneIndex == _loadingSceneIndex) //New game
            {
                _activeSceneIndex = _firstCollectSceneIndex;
            }

            if (SceneManager.GetActiveScene().buildIndex != _activeSceneIndex)
            {
                LoadSpecificScene(_activeSceneIndex);
            }
        }

        public void LoadSpecificScene(int buildIndex)
        {
            StartCoroutine(LoadScene(buildIndex));
        }

        public void RestartScene()
        {
            int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
            StartCoroutine(LoadScene(activeSceneIndex));
        }

        public void LoadBuildScene()
        {
            StartCoroutine(LoadScene(_buildSceneIndex));
        }

        public void ReturnToLastCollectScene()
        {
            StartCoroutine(LoadScene(_lastCollectSceneIndex + 1));
        }

        public void LoadFurnaceScene()
        {
            StartCoroutine(LoadScene(_furnaceSceneIndex));
        }

        IEnumerator LoadScene(int sceneIndex)
        {
            if (GameManager.Instance.GameState == GameState.Collect)
            {
                _lastCollectSceneIndex = _activeSceneIndex;
                Debug.Log("Last: " + _lastCollectSceneIndex);
            }

            _activeSceneIndex = sceneIndex;

            SaveWrapper.Instance.Save();

            _transition.SetTrigger("Start");

            yield return new WaitForSeconds(1f);

            SceneManager.LoadScene(sceneIndex);
        }

        [System.Serializable]
        struct SaveData
        {
            public int ActiveSceneIndex;
            public int LastCollectSceneIndex;
        }

        public object CaptureState()
        {
            SaveData saveData = new();
            saveData.ActiveSceneIndex = _activeSceneIndex;
            saveData.LastCollectSceneIndex = _lastCollectSceneIndex;

            return saveData;
        }

        public void RestoreState(object state)
        {
            SaveData saveData = (SaveData)state;
            _activeSceneIndex = saveData.ActiveSceneIndex;
            _lastCollectSceneIndex = saveData.LastCollectSceneIndex;
        }
    }
}
