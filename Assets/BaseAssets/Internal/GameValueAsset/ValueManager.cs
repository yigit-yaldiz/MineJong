using BaseAssets;
using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-7)]
public class ValueManager : MonoBehaviour
{
    public static ValueManager Instance { get; private set; }

    public List<Value> ValueList;

    public static Action OnUpdated;

    private void Awake()
    {
        OnUpdated = null;

        Instance = this;

        Init();

        SaveSystem.OnSave += SaveValues;
    }
    private void Init()
    {
        for (int i = 0; i < ValueList.Count; i++)
        {
            ValueList[i].Init(GetDataSave(ValueList[i].data.type));
        }
    }
    public void UpdateValue(Value.Type valueType, float value)
    {
        GetValue(valueType).Add(value);

        OnUpdated?.Invoke();
    }
    public void UpdateValue(Value value, float amount)
    {
        value.Add(amount);

        OnUpdated?.Invoke();
    }
    public void UpdateValue(Value.Type valueType, float value, Vector3 position, bool convertToScreen = true, int visualCount = 0)
    {
        Value v = GetValue(valueType);

        if (convertToScreen)
            position = MainCamera.Instance.Cam.WorldToScreenPoint(position);

        for (int i = 0; i < visualCount - 1; i++)
            UIMoveManager.Instance.StartMove(position, v.target.position, 1, 0, v.icon);

        UIMoveManager.Instance.StartMove(position, v.target.position, 1, 0, v.icon, AddValue);

        void AddValue()
        {
            UpdateValue(v, value);
        }

        OnUpdated?.Invoke();
    }
    private Value.Data GetDataSave(Value.Type valueType)
    {
        SaveSystem.SaveData.GameValues.TryGetValue(valueType, out Value.Data valueData);
        return valueData;
    }
    private void SaveValues()
    {
        for (int i = 0; i < ValueList.Count; i++)
        {
            if (ValueList[i].save)
            {
                if (SaveSystem.SaveData.GameValues.ContainsKey(ValueList[i].data.type))
                    SaveSystem.SaveData.GameValues[ValueList[i].data.type] = ValueList[i].data;
                else
                    SaveSystem.SaveData.GameValues.TryAdd(ValueList[i].data.type, ValueList[i].data);
            }
        }
    }
    public Value.Data GetData(Value.Type valueType)
    {
        for (int i = 0; i < ValueList.Count; i++)
        {
            if (ValueList[i].data.type == valueType)
                return ValueList[i].data;
        }

        return null;
    }
    public Value GetValue(Value.Type valueType)
    {
        for (int i = 0; i < ValueList.Count; i++)
        {
            if (ValueList[i].data.type == valueType)
                return ValueList[i];
        }

        return null;
    }
    public float GetCurrent(Value.Type valueType)
    {
        for (int i = 0; i < ValueList.Count; i++)
        {
            if (ValueList[i].data.type == valueType)
                return ValueList[i].data.current;
        }

        return -1;
    }
}
