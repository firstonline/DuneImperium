using Cysharp.Threading.Tasks;
using UniDi;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CouncilSeatBehaviour : MonoBehaviour
{
    [Inject] NetworkGameplayService _gameplayService;
    [SerializeField] Image[] _councilSeats;
    CompositeDisposable _disposables = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        for (int i = 0; i < _councilSeats.Length; i++)
        {
            int index = i;
            _gameplayService
                .ObservePlayerData(i)
                .Select(x => x.HasCouncilSeat)
                .Subscribe(HasCouncilSeat => _councilSeats[index].color = HasCouncilSeat ? Color.red : Color.white)
                .AddTo(_disposables);
        }
    }

    private void OnDestroy()
    {
        _disposables.Clear();
    }

}
