using BaseAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelText : MonoBehaviour
{
    public int additionalLevel;
    public string additionalText;
    public bool addToEnd;

    void Start()
    {
        string text;
        int lvl = SaveSystem.SaveData.Level + 1 + additionalLevel;
        if (addToEnd)
            text = lvl + additionalText;
        else
            text = additionalText + lvl;

        if (TryGetComponent(out Text legacyText))
        {
            legacyText.text = text;
        }
        else if (TryGetComponent(out TextMeshProUGUI tmpText))
        {
            tmpText.text = text;
        }
    }
}
