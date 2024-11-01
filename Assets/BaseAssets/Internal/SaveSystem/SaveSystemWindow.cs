using System.IO;
using UnityEditor;

#if UNITY_EDITOR
public class SaveSystemWindow : EditorWindow
{
    [MenuItem("BaseAssets/SaveSystem/Save")]
    public static void ShowSaveWindow()
    {
        SaveSystem.Save();
    }

    [MenuItem("BaseAssets/SaveSystem/Delete")]
    public static void ShowDeleteWindow()
    {
        File.Delete(SaveSystem.SavePath);
    }
}
#endif
