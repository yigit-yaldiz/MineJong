using System.Text;
using TMPro;
using UnityEngine;

public class ValueText : MonoBehaviour
{
    public enum Type { Current, Earned, Spend };
    [SerializeField] private Type type;

    [SerializeField] private Value.Type valueType;
    [SerializeField] private bool simplifyText;

    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        UpdateText();
        ValueManager.OnUpdated += UpdateText;
    }
    private void OnDisable()
    {
        ValueManager.OnUpdated -= UpdateText;
    }

    private void UpdateText()
    {
        float value = GetValue();

        if (simplifyText)
            textMeshPro.text = HandleText(value);
        else
            textMeshPro.text = value.ToString();
    }

    private float GetValue()
    {
        Value value = ValueManager.Instance.GetValue(valueType);
        if (value == null)
            return 0;

        switch (type)
        {
            case Type.Current:
                return value.data.current;
            case Type.Earned:
                return value.data.earned;
            case Type.Spend:
                return value.data.spend;
            default:
                return 0;
        }
    }


    private string HandleText(float value)
    {
        StringBuilder stringBuilder = new StringBuilder();
        int v = Mathf.CeilToInt(value);

        if (value < 1000)
        {
            stringBuilder.Append(v);
        }
        else if (value < 100000)
        {
            stringBuilder.Append(v / 1000).Append(".").Append(v % 1000 / 100).Append("k");
        }
        else
        {
            stringBuilder.Append(v / 1000000).Append(".").Append(v % 1000000 / 100).Append("m");
        }

        return stringBuilder.ToString();
    }
}
