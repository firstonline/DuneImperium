using Cysharp.Threading.Tasks;
using UniDi;
using UniRx;
using UnityEngine;

public class PlayerHandBehaviour : MonoBehaviour
{
    [Inject] NetworkGameplayService _gameplayService;
    [Inject] CardsDatabase _cardsDatabase;

    [SerializeField] CardBehaviour _cardPrefab;
    [SerializeField] Transform _cardsParent;

    PrefabsPool<CardBehaviour> _cardsPool;
    CompositeDisposable _disposables = new();

    CardBehaviour _selectedCard;
    public CardDefinition SelectedCard => _selectedCard?.CardDefinition;

    void Awake()
    {
        _cardsPool = new PrefabsPool<CardBehaviour>(_cardPrefab, _cardsParent);
        _cardPrefab.gameObject.SetActive(false);
        _gameplayService.ObserveLocalPlayerData().Subscribe(x =>
        {
            _cardsPool.ReleaseAll();
            _selectedCard = null;

            for (int i = 0; i < x.Cards.Count; i++)
            {
                var card = _cardsPool.Get();
                card.Setup(_cardsDatabase.GetItem(x.Cards[i]), OnCardSelected);
                card.transform.SetAsFirstSibling();
            }
        }).AddTo(_disposables);
    }

    void OnCardSelected(CardBehaviour cardBehaviour)
    {
        if (_selectedCard != null)
        {
            _selectedCard.SetIsSelected(false);
        }
        _selectedCard = cardBehaviour;
        _selectedCard.SetIsSelected(true);
    }

    void OnDestroy()
    {
        _disposables.Clear();
    }
}
