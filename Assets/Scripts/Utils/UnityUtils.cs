using System.Collections;
using UnityEngine;

public static class UnityUtils
{
    public static void HideAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(false);
        }
    }
}
