using UnityEngine;

public class HideFlagChecker : MonoBehaviour
{
    void Start()
    {
        Object[] allObjects = Resources.FindObjectsOfTypeAll<Object>();

        foreach (var obj in allObjects)
        {
            if ((obj.hideFlags & HideFlags.DontSaveInEditor) != 0)
            {
                Debug.LogWarning($"!オブジェクト '{obj.name}' に HideFlags.DontSaveInEditor が付いています！ タイプ: {obj.GetType()}", obj);
            }
        }
    }
}
