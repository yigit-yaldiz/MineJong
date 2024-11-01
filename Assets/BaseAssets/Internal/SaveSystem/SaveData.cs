using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    private int level;

    private Dictionary<Value.Type, Value.Data> gameValues = new Dictionary<Value.Type, Value.Data>();

    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
        }
    }
    public Dictionary<Value.Type, Value.Data> GameValues
    {
        get
        {
            return gameValues;
        }
    }
}
