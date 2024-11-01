using UnityEngine;

[System.Serializable]
public class Value
{
    public enum Type { Money };

    public Data data;
    public bool save;

    public Transform target;
    public Sprite icon;

    [System.Serializable]
    public class Data
    {
        public Type type;

        public float current;
        public float earned;
        public float spend;
        public float multiplier = 1;
    }

    public void Init(Data valueData)
    {
        data.current = valueData != null ? valueData.current : 0;
        data.earned = 0;
        data.spend = 0;
    }

    public void Add(float value)
    {
        if (value > 0)
            value *= data.multiplier;
        data.current += value;
        if (value > 0)
            data.earned += value;
        else
            data.spend += value;
    }
}