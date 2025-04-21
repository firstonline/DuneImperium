using System.Collections;
using System.Linq;
using UniDi;
using UniRx;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{
    [Inject] NetworkGameplayService networkGameplayService;

    [SerializeField] BasicResourceUI _basicResourcePrefab;
    CompositeDisposable _disposables = new();

    void Awake()
    {
    }

    void OnDestroy()
    {
        _disposables.Clear();
    }

    public void Setup(int playerIndex)
    {
        _disposables.Clear();

        networkGameplayService.ObservePlayerData(playerIndex).Subscribe(Setup).AddTo(_disposables);
    }

    void Setup(PlayerData playerData)
    {
        
    }
}
