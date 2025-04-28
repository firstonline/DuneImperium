using System.Collections;
using System.Linq;
using TMPro;
using UniDi;
using UniRx;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{
    [Inject] NetworkGameplayService networkGameplayService;

    [SerializeField] TextMeshProUGUI _water;
    [SerializeField] TextMeshProUGUI _solari;
    [SerializeField] TextMeshProUGUI _spice;
    [SerializeField] TextMeshProUGUI _availableAgents;

    CompositeDisposable _disposables = new();

    void Awake()
    {
    }

    void OnDestroy()
    {
        _disposables.Clear();
    }

    public void Setup(int playerIndex)
    {
        Debug.Log($"Creating{playerIndex}");
        _disposables.Clear();
        _playerIndex = playerIndex;
        networkGameplayService.ObservePlayerData(playerIndex).Subscribe(Setup).AddTo(_disposables);
    }

    int _playerIndex;
    void Setup(PlayerData playerData)
    {
        _water.text = playerData.Resources[ResourceType.Water].ToString();
        _solari.text = playerData.Resources[ResourceType.Solari].ToString();
        _spice.text = playerData.Resources[ResourceType.Spice].ToString();
        _availableAgents.text = (playerData.AgentsCount - playerData.DeployedAgentsCount).ToString();
    }
}
