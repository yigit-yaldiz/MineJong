using UnityEngine;

public class UIButtonMethods : MonoBehaviour
{
    public void PlayButton()
    {
        GameManager.Instance.Play();
    }
    public void NextLevel()
    {
        GameManager.Instance.NextLevel();
    }
    public void Reload()
    {
        GameManager.Instance.Reload();
    }
}
