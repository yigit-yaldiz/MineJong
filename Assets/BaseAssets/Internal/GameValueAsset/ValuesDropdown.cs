using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ValuesDropdown : MonoBehaviour
{
    TMP_Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> optionDataList = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < ValueManager.Instance.ValueList.Count; i++)
        {
            TMP_Dropdown.OptionData newOption = new TMP_Dropdown.OptionData();
            Value v = ValueManager.Instance.GetValue((Value.Type)i);
            newOption.text = v.data.type.ToString();
            newOption.image = v.icon;
            optionDataList.Add(newOption);
        }
        dropdown.AddOptions(optionDataList);
    }
}
