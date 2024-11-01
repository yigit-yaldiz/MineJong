using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniqueGames.Saving;
using BaseAssets;

namespace Saving
{
    [RequireComponent(typeof(SavingSystem))]
    public class SaveWrapper : MonoBehaviour
    {
        [System.NonSerialized] private string _undisposableSaveFile = "Undisposable";
        [System.NonSerialized] private string _disposableSaveFile = "Disposable";
        private SavingSystem _savingSystem;

        public static SaveWrapper Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            _savingSystem = GetComponent<SavingSystem>();
            _savingSystem.Load(_undisposableSaveFile);
            _savingSystem.Load(_disposableSaveFile);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftAlt))
            {
                Save();
            }
        }

        public void Save()
        {
            _savingSystem.Save(_undisposableSaveFile, false);
            _savingSystem.Save(_disposableSaveFile, true);
        }

        public void DeleteDisposable()
        {
            _savingSystem.DeleteSave(_disposableSaveFile);
        }

        [Button]
        public void DeleteDisposables()
        {
            _savingSystem = GetComponent<SavingSystem>();
            _savingSystem.DeleteSave(_disposableSaveFile);
        }
    }
}
