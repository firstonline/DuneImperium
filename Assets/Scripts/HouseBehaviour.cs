using NaughtyAttributes;
using System;
using System.Collections;
using UniDi;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HouseBehaviour : MonoBehaviour
{
    [Inject] NetworkGameplayService _gameplayService;

    [SerializeField] ResourceType _influence;
    [SerializeField] GameObject[] _influenceSteps;
    [SerializeField] Image[] _players;

    CompositeDisposable _disposables = new();

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            int index = i;

            _gameplayService.ObservePlayerData(index)
                .Skip(1)
                .Select(x => x.Resoureces[_influence])
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    var currentPosition = _players[index].transform.position;
                    currentPosition.y = _influenceSteps[x].transform.position.y;
                    _players[index].transform.position = currentPosition;
                })
                .AddTo(_disposables);
        }
    }

    void OnDestroy()
    {
        _disposables.Clear();
    }

    public int testLadder = 0;

    [Button]
    public void Testing()
    {
        for (int i = 0; i < 4; i++)
        {
            var currentPosition = _players[i].transform.position;
            currentPosition.y = _influenceSteps[testLadder].transform.position.y;
            _players[i].transform.position = currentPosition;
        }
    }
}
