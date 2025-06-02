using UniDi;
using UniRx;
using UnityEngine;

public class GameBoardSpawner : MonoBehaviour
{
    [Inject] DiContainer _cointainer;
    [Inject] BehaviorSubject<GameData> _gameData;

    [SerializeField] GameObject _board;
    CompositeDisposable _disposables = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameData.Subscribe(x =>
        {
            if (x.Players != null)
            {
                _cointainer.InstantiatePrefab(_board);
                _disposables.Clear();
            }
        }).AddTo(_disposables);
    }

    void OnDestroy()
    {
        _disposables.Clear();
    }
}
