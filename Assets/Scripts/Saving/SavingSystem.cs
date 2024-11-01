using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UniqueGames.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        SaveableEntity[] _saveableEntities;
        private void Awake()
        {
            _saveableEntities = FindObjectsOfType<SaveableEntity>(true);
        }
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                int buildIndex = (int)state["lastSceneBuildIndex"];
                if (buildIndex != SceneManager.GetActiveScene().buildIndex)
                {
                    yield return SceneManager.LoadSceneAsync(buildIndex);
                }
            }
            RestoreState(state);
        }

        public void Save(string saveFile, bool isDisposable)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state, isDisposable);
            SaveFile(saveFile, state);
        }

        public void DeleteSave(string saveFile)
        {
            DeleteSaveFile(saveFile);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private void DeleteSaveFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            if (File.Exists(path))
            {
                File.Delete(path);
                print("Save file deleted: " + path);
            }
            else
            {
                print("Save file not found: " + path);
            }
        }

        private void CaptureState(Dictionary<string, object> state, bool isDisposable)
        {
            foreach (SaveableEntity saveable in _saveableEntities)
            {
                if (isDisposable == saveable.IsDisposable)
                {
                    state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
                }
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            if(_saveableEntities.Length> 0)
            {
                foreach (SaveableEntity saveable in _saveableEntities)
                {

                    string id = saveable.GetUniqueIdentifier();
                    if (state.ContainsKey(id))
                    {
                        saveable.RestoreState(state[id]);
                    }
                }
            }
            else
            {

                foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>(true))
                {
                    string id = saveable.GetUniqueIdentifier();
                    if (state.ContainsKey(id))
                    {
                        saveable.RestoreState(state[id]);
                    }
                }
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}