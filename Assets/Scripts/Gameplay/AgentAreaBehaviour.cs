using TMPro;
using UniDi;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AgentArea : MonoBehaviour
{
    [SerializeField] AgentAreaDefinition _definition;
    [SerializeField] TextMeshProUGUI _name;

    [Inject] NetworkGameplayService _gameplayService;

    CompositeDisposable _disposables = new();
    Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.OnClickAsObservable().Subscribe(x =>
        {
            _gameplayService.VisitAgentArea((int)NetworkManager.Singleton.LocalClientId, _definition.ID);
        }).AddTo(_disposables);

        _name.text = _definition.Name;
    }

    private void OnDestroy()
    {
        _disposables.Clear();
    }

}
