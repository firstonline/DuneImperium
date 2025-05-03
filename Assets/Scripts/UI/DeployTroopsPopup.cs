using System;
using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DeployTroopsPopup : MonoBehaviour
{
    [SerializeField] Button _increase;
    [SerializeField] Button _decrease;
    [SerializeField] Button _confirm;
    [SerializeField] TextMeshProUGUI _count;

    CompositeDisposable _disposables = new();
    Action<int> _onConfirm;

    int _maxTroopsCount;
    int _troopsCount;

    void Awake()
    {
        _confirm.OnClickAsObservable().Subscribe(_ =>
        {
            _onConfirm?.Invoke(_troopsCount);
            gameObject.SetActive(false);
        }).AddTo(_disposables);

        _increase.OnClickAsObservable().Subscribe(_ =>
            {
                _troopsCount = Mathf.Clamp(_troopsCount + 1, 0, _maxTroopsCount);
                UpdateCount();
            }).AddTo(_disposables);

        _decrease.OnClickAsObservable().Subscribe(_ =>
            {
                _troopsCount = Mathf.Clamp(_troopsCount - 1, 0, _maxTroopsCount);
                UpdateCount();
            }).AddTo(_disposables);
    }

    void OnDestroy()
    {
        _disposables.Clear();
    }

    public void Show(Action<int> onConfirm, int deployableTroopsCount)
    {
        _onConfirm = onConfirm;
        _troopsCount = 0;
        _maxTroopsCount = deployableTroopsCount;
        UpdateCount();
        gameObject.SetActive(true);
    }

    void UpdateCount()
    {
        _count.text = _troopsCount.ToString();
    }
}
