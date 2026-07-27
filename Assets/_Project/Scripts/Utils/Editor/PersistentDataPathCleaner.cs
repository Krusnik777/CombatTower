using UnityEditor;
using UnityEngine;
using System.IO;

namespace Utils.Editor
{
    public static class PersistentDataPathCleaner
    {
        [MenuItem("Tools/Delete All Files In Persistent Data Path", false, 0)]
        private static void DeleteAllFilesInPersistentDataPath()
        {
            //string dataPath = Path.Combine(Application.persistentDataPath, "Saves");
            string dataPath = Application.persistentDataPath;

            if (Directory.Exists(dataPath))
            {
                bool confirm = EditorUtility.DisplayDialog(
                    "Delete All Saves",
                    $"Are you sure you want delete saves?\nPath: {dataPath}",
                    "Delete",
                    "Cancel"
                );

                if (!confirm) return;

                Directory.Delete(dataPath, true);
                Directory.CreateDirectory(dataPath);

                Debug.Log($"<color=green>Saves deleted:</color> {dataPath}");

                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Save folder doesn't exist: " + dataPath);
            }

            PlayerPrefs.DeleteAll();
            Debug.Log("<color=orange>Player Prefs Also Cleared!</color>");
        }
    }
}
