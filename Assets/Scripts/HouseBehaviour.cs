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
        Invoke("Setup", 0.1f);
    }

    void OnDestroy()
    {
        _disposables.Clear();
    }

    public void Setup()
    {
        Debug.Log($"====Setting up player influcences {_influence.ToString()}");
        for (int i = 0; i < 4; i++)
        {
            int index = i;

            _gameplayService.ObservePlayerData(index)
                .Select(x => x.Resources[_influence])
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    var currentPosition = _players[index].transform.position;
                    currentPosition.y = _influenceSteps[x].transform.position.y;
                    _players[index].transform.position = currentPosition;
                    Debug.Log($"Setting up player influcence {index} {x}");
                })
                .AddTo(_disposables);
        }
    }
}
