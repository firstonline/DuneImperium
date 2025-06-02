using Cysharp.Threading.Tasks;
using System.Threading;
using UniDi;
using UniRx;
using Unity.Netcode;

public abstract class PhaseServiceBase : NetworkBehaviour
{
    [Inject] BehaviorSubject<GameData> _gameData;

    public abstract UniTask Process(CancellationTokenSource cancellationTokenSource);
}