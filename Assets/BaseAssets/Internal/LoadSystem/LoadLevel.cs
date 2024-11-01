using UnityEngine;
using UnityEngine.UI;

namespace BaseAssets
{
    public class LoadLevel : MonoBehaviour
    {
        [SerializeField] private Image fill;
        AsyncOperation asyncOperation;
        bool fade;

        private void Start()
        {
            asyncOperation = LoadSystem.LoadSceneAsync(LevelSettings.Instance.SceneIndex);
            asyncOperation.allowSceneActivation = false;
        }

        private void Update()
        {
            fill.fillAmount = asyncOperation.progress;
            if (asyncOperation.progress >= .9f && !fade)
            {
                fade = true;
                FadeScript.Instance.Fade(true, delegate { asyncOperation.allowSceneActivation = true; });
            }
        }
    }
}
