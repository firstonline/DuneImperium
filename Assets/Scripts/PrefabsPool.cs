using System.Collections.Generic;
using UniDi;
using UnityEngine;
using UnityEngine.Pool;

public class PrefabsPool<T> where T : UnityEngine.Object
{
    T _prefab;
    Transform _parent;
    int _capacity;
    DiContainer _container;
    List<T> _activeItems;

    ObjectPool<T> _pool;

    public PrefabsPool(T prefab, Transform parent, int capacity, DiContainer container)
    {
        _container = container;
        _prefab = prefab;
        _parent = parent;
        _capacity = capacity;

        _activeItems = new();

        _pool = new ObjectPool<T>
            (
                createFunc: () =>
                {
                    if (_prefab is Component component)
                    {
                        return _container.InstantiatePrefabForComponent<T>(_prefab, _parent);
                    }
                    else if (_prefab is GameObject go)
                    {
                        return _container.InstantiatePrefab(go, _parent) as T;
                    }
                    return default;
                },
                actionOnGet: (objectToGet) =>
                {
                    if (objectToGet is Component component)
                    {
                        component.gameObject.SetActive(true);
                    }
                    else if (objectToGet is GameObject go)
                    {
                        go.SetActive(true);
                    }
                    _activeItems.Add(objectToGet);
                },
                actionOnRelease: (objectToRelease) =>
                {
                    if (objectToRelease is Component component)
                    {
                        component.gameObject.SetActive(false);
                        component.transform.SetParent(_parent);
                    }
                    else if (objectToRelease is GameObject go)
                    {
                        go.SetActive(false);
                        go.transform.SetParent(_parent);
                    }
                    _activeItems.Remove(objectToRelease);
                }
            );
    }

    public T Get()
    {
        return _pool.Get();
    }

    public void Release(T item)
    {
        _pool.Release(item);
    }

    public void ReleaseAll()
    {
        for (int i = _activeItems.Count - 1; i >= 0; i--)
        {
            Release(_activeItems[i]);
        }
    }

}
