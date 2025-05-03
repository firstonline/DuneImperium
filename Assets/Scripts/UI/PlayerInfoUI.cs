using TMPro;
using UniDi;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    [Inject] NetworkGameplayService networkGameplayService;

    [SerializeField] TextMeshProUGUI _water;
    [SerializeField] TextMeshProUGUI _solari;
    [SerializeField] TextMeshProUGUI _spice;
    [SerializeField] TextMeshProUGUI _availableAgents;
    [SerializeField] Image _fremenAllianceToken;
    [SerializeField] Image _benegesseritAllianceToken;
    [SerializeField] Image _spacingGuildAllianceToken;
    [SerializeField] Image _emperrorAllianceToken;

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

        _fremenAllianceToken.color = playerData.Alliances[House.Fremen] ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        _benegesseritAllianceToken.color = playerData.Alliances[House.BeneGesserit] ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        _spacingGuildAllianceToken.color = playerData.Alliances[House.SpacingGuild] ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        _emperrorAllianceToken.color = playerData.Alliances[House.Emperror] ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }
}
