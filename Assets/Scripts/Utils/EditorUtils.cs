using System.Collections;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

public static class EditorUtils
{
    public static void SetDirty(UnityEngine.Object go)
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            return;
        }
        EditorUtility.SetDirty(go);
#endif
    }

}
