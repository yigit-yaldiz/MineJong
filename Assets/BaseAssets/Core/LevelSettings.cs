using UnityEngine;
using UnityEngine.SceneManagement;

namespace BaseAssets
{
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "BaseAssets/LevelSettings")]
    public class LevelSettings : SingletonScriptableObject<LevelSettings>
    {
        public int LoopStartLevel = 1;
        public int SceneIndex
        {
            get
            {
                int loopIndex = LoopStartLevel - 1;
                int levelSceneCount = SceneManager.sceneCountInBuildSettings - 1;
                bool firstLoopCompleted = SaveSystem.SaveData.Level >= levelSceneCount;
                int index;
                int offset = Mathf.Clamp(loopIndex, 0, levelSceneCount);
                if (firstLoopCompleted)
                    index = (SaveSystem.SaveData.Level - levelSceneCount) % (levelSceneCount - offset) + offset;
                else
                    index = SaveSystem.SaveData.Level % levelSceneCount;

                return index + 1;
            }
        }
    }
}