using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingAnim : MonoBehaviour
{
    TMP_Text _loadingText;
    const float _interval = 0.25f;

    private void Awake()
    {
        _loadingText = GetComponent<TMP_Text>();
    }

    private IEnumerator Start()
    {
        while (true)
        {
            _loadingText.text = "Loading";
            yield return new WaitForSeconds(_interval);

            _loadingText.text = "Loading.";
            yield return new WaitForSeconds(_interval);

            _loadingText.text = "Loading..";
            yield return new WaitForSeconds(_interval);

            _loadingText.text = "Loading...";
            yield return new WaitForSeconds(_interval);
        }
    }
}
