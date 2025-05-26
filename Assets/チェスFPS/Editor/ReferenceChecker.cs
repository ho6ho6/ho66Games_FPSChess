#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class ReferenceChecker : EditorWindow
{
    [MenuItem("Tools/チェック: DontSaveInEditorな参照を探す")]
    public static void CheckReferences()
    {
        var allBehaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (var script in allBehaviours)
        {
            var so = new SerializedObject(script);
            var prop = so.GetIterator();

            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference)
                {
                    var refObj = prop.objectReferenceValue;
                    if (refObj != null && (refObj.hideFlags & HideFlags.DontSaveInEditor) != 0)
                    {
                        Debug.LogWarning($"⚠ {script.name}（{script.GetType()}）のフィールド {prop.displayName} に保存不可なオブジェクト {refObj.name}（{refObj.GetType()}） がアサインされています", script);
                    }
                }
            }
        }
    }
}
#endif