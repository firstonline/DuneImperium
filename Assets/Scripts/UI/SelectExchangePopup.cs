using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SelectExchangePopup : MonoBehaviour
{
    [SerializeField] Button _option1Btn;
    [SerializeField] Button _option2Btn;

    CompositeDisposable _disposables = new();
    Action<int> _onChoiceSelected;

    void Awake()
    {
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

    public void Show(Action<int> onChoiceSelected)
    {
        gameObject.SetActive(true);
        _onChoiceSelected = onChoiceSelected;
    }
}
