using System;
using System.Collections.Generic;
using System.Linq;
using UniDi;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SelectExchangePopup : MonoBehaviour
{
    [Inject] GameplayService _gameplayService;

    [SerializeField] Button _closeButton;
    [SerializeField] Button _option1Btn;
    [SerializeField] ExchangeBehaviour _exchange1Behaviour;
    [SerializeField] Button _option2Btn;

    [SerializeField] ExchangeBehaviour _exchange2Behaviour;

    CompositeDisposable _disposables = new();
    Action<int> _onChoiceSelected;

    void Awake()
    {
        _closeButton.OnClickAsObservable().Subscribe(_ => gameObject.SetActive(false));
        _option1Btn.OnClickAsObservable().Subscribe(x =>
        {
            _onChoiceSelected?.Invoke(0);
            gameObject.SetActive(false);
        }).AddTo(_disposables);
        _option2Btn.OnClickAsObservable().Subscribe(x =>
        {
            _onChoiceSelected?.Invoke(1);
            gameObject.SetActive(false);
        }).AddTo(_disposables);
    }

    public void Show(List<ExchangeDefinition> exchanges, Action<int> onChoiceSelected)
    {
        gameObject.SetActive(true);
        _exchange1Behaviour.Setup(exchanges[0]);
        _exchange2Behaviour.Setup(exchanges[1]);

        SetupButton(_option1Btn, exchanges[0]);
        SetupButton(_option2Btn, exchanges[1]);

        _onChoiceSelected = onChoiceSelected;
    }

    void SetupButton(Button button, ExchangeDefinition exchange)
    {
        var gameData = _gameplayService.GameData;

        var playerData = gameData.Players.First(x => x.ClientId == NetworkManager.Singleton.LocalClientId);

        button.interactable = ExchangeHelper.IsExchangeValid(gameData, playerData, exchange);
    }
}
