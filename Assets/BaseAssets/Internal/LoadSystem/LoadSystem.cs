using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSystem
{
    public static void Reload(bool fade)
    {
        int index = SceneManager.GetActiveScene().buildIndex;

        Load(index, fade);
    }
    public static void LoadScene(int sceneIndex, bool fade)
    {
        //int index = level % (SceneManager.sceneCountInBuildSettings - 1);

        Load(sceneIndex, fade);
    }
    public static AsyncOperation LoadSceneAsync(int sceneIndex)
    {
        //int index = level % (SceneManager.sceneCountInBuildSettings - 1);

        return SceneManager.LoadSceneAsync(sceneIndex);
    }

    private static void Load(int index, bool fade)
    {
        if (!fade)
            SceneManager.LoadScene(index);
        else
        {
            if (FadeScript.Instance)
                FadeScript.Instance.Fade(true, delegate { SceneManager.LoadScene(index); });
        }
    }
}
