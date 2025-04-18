using System;
using System.Linq;
using TMPro;
using UniDi;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayersPanelUI : MonoBehaviour
{
    [Inject] DiContainer _container;
    [SerializeField] PlayerInfoUI _playerInfoPrefab;

    PrefabsPool<PlayerInfoUI> _playerInfoPool;

    void Awake()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        _playerInfoPool = new(_playerInfoPrefab, this.transform, 10, _container);

        for (int i = 0; i < 4; i++)
        {
            var playerInfoUI = _playerInfoPool.Get();
            playerInfoUI.Setup(i);
        }
    }
}
