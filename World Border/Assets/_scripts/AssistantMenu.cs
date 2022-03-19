using UnityEditor;
using UnityEngine;

public class AssistantMenu : EditorWindow
{
    [MenuItem("Edit/ Clear Player Prefs")]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
