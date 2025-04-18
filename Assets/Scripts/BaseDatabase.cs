using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class BaseDatabase<T> : ScriptableObject where T : UnityEngine.Object, IDatabaseItem
{
    Dictionary<int, T> _dictionaryData;

    public List<T> Items;

    public T GetItem(int id)
    {
        if (_dictionaryData.ContainsKey(id))
            return _dictionaryData[id];

        return default;
    }

    public void Setup()
    {
        _dictionaryData = new Dictionary<int, T>();
        foreach (var item in Items)
        {
            _dictionaryData.Add(item.ID, item);
        }
    }

#if UNITY_EDITOR
    [Button]
    public void AutoSetup()
    {
        var itemsGUIDS = AssetDatabase.FindAssets($"t: {typeof(T).Name}");
        Items = new List<T>();
        foreach (var itemGUID in itemsGUIDS)
        {
            var itemPath = AssetDatabase.GUIDToAssetPath(itemGUID);
            var item = AssetDatabase.LoadAssetAtPath<T>(itemPath);
            Items.Add(item);
        }

        Items = Items.OrderBy(x => x.name).ToList();

        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i]; 
            item.ID = i;
            EditorUtility.SetDirty(item);
        }
       

        EditorUtility.SetDirty(this);
    }
#endif
}
