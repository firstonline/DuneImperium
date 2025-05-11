using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] Button _btn;
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] Image _icon;
    [SerializeField] Image _agentIconPrefab;
    [SerializeField] Image _selectedImage;
    [SerializeField] Transform _agentIconsParent;
    [SerializeField] ExchangeBehaviour _playExchangeBehaviour;
    [SerializeField] ExchangeBehaviour _revealExchangeBehaviour;

    PrefabsPool<Image> _agentIconsPool;
    Action<CardBehaviour> _onCardSelected;
    CompositeDisposable _disposables = new();

    public CardDefinition CardDefinition { get; private set; }

    void Awake()
    {
        _agentIconsPool = new PrefabsPool<Image>(_agentIconPrefab, _agentIconsParent);
        _agentIconPrefab.gameObject.SetActive(false);
        _btn.OnClickAsObservable().Subscribe(_ => _onCardSelected?.Invoke(this)).AddTo(_disposables);
    }

    void OnDestroy()
    {
        _disposables.Clear();
    }

    public void Setup(CardDefinition definition, Action<CardBehaviour> onCardSelected)
    {
        CardDefinition = definition;
        _agentIconsPool.ReleaseAll();
        _icon.sprite = CardDefinition.Icon;
        _nameText.text = CardDefinition.Name;
        _onCardSelected = onCardSelected;
        SetIsSelected(false);

        _playExchangeBehaviour.Setup(definition.PlayExchanges);
        _revealExchangeBehaviour.Setup(definition.RevealExchange);

        for (int i = 0; i < CardDefinition.AgentIcons.Count; i++)
        {
            var agentIcon = _agentIconsPool.Get();
            agentIcon.sprite = CardDefinition.AgentIcons[i].Icon;
        }
    }

    public void SetIsSelected(bool isSelected)
    {
        _selectedImage.gameObject.SetActive(isSelected);
    }
}
