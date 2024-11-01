using BaseAssets;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    public static FadeScript Instance { get; private set; }

    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        fadeImage.color = Color.black;
        Fade(false);
    }

    public void Fade(bool fadeToBlack, Action onComplete = null)
    {
        fadeImage.raycastTarget = true;
        StartCoroutine(FadeRoutine());

        IEnumerator FadeRoutine()
        {
            Color startColor = fadeToBlack ? Color.clear : Color.black;
            Color endColor = fadeToBlack ? Color.black : Color.clear;
            float elapsedTime = 0;
            while (elapsedTime < Settings.Instance.fadeTime)
            {
                fadeImage.color = Color.Lerp(startColor, endColor, elapsedTime / Settings.Instance.fadeTime);
                elapsedTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            fadeImage.raycastTarget = false;
            fadeImage.color = endColor;
            onComplete?.Invoke();
        }
    }
}
