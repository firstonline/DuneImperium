using Cysharp.Threading.Tasks;
using TMPro;
using UniDi;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombatBehaviour : MonoBehaviour
{
    [Inject] NetworkGameplayService _gameplayService;

    [SerializeField] int _playerIndex;
    [SerializeField] Image _playerIcon;
    [SerializeField] GameObject[] _deployedTroops;
    [SerializeField] GameObject[] _worms;
    [SerializeField] TextMeshProUGUI _garrisonedTroopsText;
    [SerializeField] TextMeshProUGUI _combatStrengthText;

    CompositeDisposable _disposables = new();

    void Awake()
    {
        _gameplayService.ObservePlayerData(_playerIndex).Subscribe(x =>
        {
            UnityUtils.HideAllChildren(_deployedTroops[0].transform.parent);
            UnityUtils.HideAllChildren(_worms[0].transform.parent);

            _garrisonedTroopsText.text = x.GarrisonedTroopsCount.ToString();
            _combatStrengthText.text = x.CombatStrength.ToString();
            for (int i = 0; i < x.DeployedTroopsCount; i++)
            {
                _deployedTroops[i].SetActive(true);
            }

            for (int i = 0; i < x.WormsCount && i < _worms.Length; i++)
            {
                _worms[i].SetActive(true);
            }
        }).AddTo(_disposables);
    }

    void OnDestroy()
    {
        _disposables.Clear();
    }
}
