using UniDi;
using UnityEngine;

public class PlayersPanelUI : MonoBehaviour
{
    [Inject] DiContainer _container;
    [Inject] NetworkGameplayService _gameplayService;
    [SerializeField] PlayerInfoUI _playerInfoPrefab;

    PrefabsPool<PlayerInfoUI> _playerInfoPool;

    void Awake()
    {
        UnityUtils.HideAllChildren(this.transform);

        _playerInfoPool = new(_playerInfoPrefab, this.transform, 10);

        for (int i = 0; i < _gameplayService.PlayersCount; i++)
        {
            var playerInfoUI = _playerInfoPool.Get();
            _container.Inject(playerInfoUI);
            playerInfoUI.Setup(i);
        }
    }
}
