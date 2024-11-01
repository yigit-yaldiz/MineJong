using BaseAssets;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem
{
    private static SaveData saveData = new SaveData();
    public static SaveData SaveData
    {
        get
        {
            if (!loaded)
                Load();
            return saveData;
        }
    }

    public static Action OnSave;

    public static string SavePath => Application.persistentDataPath + "/save.data";
    private static bool loaded;

    public static void Load()
    {
        OnSave = null;

        if (File.Exists(SavePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(SavePath, FileMode.Open);

            saveData = binaryFormatter.Deserialize(fileStream) as SaveData;
            fileStream.Close();
        }
        else
        {
            saveData = new SaveData();
        }
        loaded = true;

        Debugger.Log("Loaded");
    }
    public static void Save()
    {
        OnSave?.Invoke();

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(SavePath, FileMode.Create);

        binaryFormatter.Serialize(fileStream, SaveData != null ? SaveData : new SaveData());
        fileStream.Close();

        Debugger.Log("Saved");
    }
    public static void Delete()
    {
        File.Delete(SavePath);
        Load();
        Save();

        Debugger.Log("Save Deleted");
    }
}
