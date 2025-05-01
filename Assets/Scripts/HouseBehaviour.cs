using NaughtyAttributes;
using System;
using System.Collections;
using System.Linq;
using UniDi;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HouseBehaviour : MonoBehaviour
{
    [Inject] NetworkGameplayService _gameplayService;

    [SerializeField] Image _houseIcon;
    [SerializeField] House _house;
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
        for (int i = 0; i < 4; i++)
        {
            int index = i;

            _gameplayService.ObservePlayerData(index)
                .Select(x => x.Influences[_house])
                .DistinctUntilChanged()
                .Subscribe(x =>
                {
                    var currentPosition = _players[index].transform.position;
                    currentPosition.y = _influenceSteps[x].transform.position.y;
                    _players[index].transform.position = currentPosition;
                })
                .AddTo(_disposables);

            _gameplayService
                .ObserveGameData()
                .Select(x => x.Players.Any(x => x.Alliances[_house]))
                .DistinctUntilChanged()
                .Subscribe(hasAlliance =>
                {
                    _houseIcon.color = hasAlliance ? new Color(1, 1, 1, 0.5f) : Color.white;
                })
                .AddTo(_disposables);
        }
    }
}
