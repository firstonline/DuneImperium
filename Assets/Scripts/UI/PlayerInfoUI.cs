using System.Collections;
using System.Linq;
using UniDi;
using UniRx;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{

    [Inject] NetworkGameplayService networkGameplayService;
    [Inject] ResourcesDatabase _resourcesDatabase;

    [SerializeField] BasicResourceUI _water;
    [SerializeField] BasicResourceUI _spice;
    [SerializeField] BasicResourceUI _solari;
    CompositeDisposable _disposables = new();

    void Awake()
    {
        _water.Icon.sprite = _resourcesDatabase.Items.First(x => x.ResourceType == ResourceType.Water).Icon;
        _spice.Icon.sprite = _resourcesDatabase.Items.First(x => x.ResourceType == ResourceType.Spice).Icon;
        _solari.Icon.sprite = _resourcesDatabase.Items.First(x => x.ResourceType == ResourceType.Solari).Icon;

        _water.Quantity.text = 0.ToString();
        _spice.Quantity.text = 0.ToString();
        _solari.Quantity.text = 0.ToString();
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
        Debug.Log("Updating Data");
        foreach (var itemKeyValPair in playerData.Items)
        {
            var item = _resourcesDatabase.GetItem(itemKeyValPair.Key);
            switch (item.ResourceType)
            {
                case ResourceType.Water:
                    _water.Quantity.text = itemKeyValPair.Value.ToString();
                    break;
                case ResourceType.Spice:
                    _spice.Quantity.text = itemKeyValPair.Value.ToString();
                    break;
                case ResourceType.Solari:
                    _solari.Quantity.text = itemKeyValPair.Value.ToString();
                    break;
            }
        }
    }
}
