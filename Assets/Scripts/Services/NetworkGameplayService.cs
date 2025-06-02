using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using UniDi;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class NetworkGameplayService : NetworkBehaviour
{
    [Inject] BehaviorSubject<GameData> _gameData;

    [SerializeField] string _connectionScreen;
    [SerializeField] Button _leaveSessionButton;

    CompositeDisposable _disposables = new();
    NetworkVariable<GameData> _networkVariable = new NetworkVariable<GameData>();

    int iterator = 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (NetworkManager.Singleton == null)
        {
            throw new Exception($"");
        }

        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            _gameData.Subscribe(updatedGameData =>
            {
                iterator++;
                updatedGameData.Iterator = iterator;
                _networkVariable.Value = updatedGameData;
                DataUpdatedClientRpc();
            }).AddTo(_disposables);
        }
        else
        {
            _gameData.OnNext(_networkVariable.Value);
           _networkVariable.OnValueChanged += (oldData, newData) =>
            {
                _gameData.OnNext(newData);
            };
        }

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
        _disposables.Clear();
    }

    [ClientRpc]
    void DataUpdatedClientRpc()
    {
    }

    void OnClientConnected(ulong clientID)
    {
        var gameData = _networkVariable.Value;

        for (int i = 0; i < gameData.Players.Count; i++)
        {
            var player = gameData.Players[i];
            if (!player.IsTaken)
            {
                player.IsTaken = true;
                player.ClientId = clientID;
                gameData.Players[i] = player;
                gameData.Iterator++;
                _networkVariable.Value = gameData;
                break;
            }
        }
    }

    void OnClientDisconnected(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            if (!NetworkManager.Singleton.IsHost && _leaveSessionButton != null)
            {
                _leaveSessionButton.onClick.Invoke();
            }
            SceneManager.LoadSceneAsync(_connectionScreen);
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            var gameData = _networkVariable.Value;

            for (int i = 0; i < gameData.Players.Count; i++)
            {
                var player = gameData.Players[i];
                if (player.ClientId == clientID)
                {
                    player.IsTaken = false;
                    gameData.Players[i] = player;
                    gameData.Iterator++;
                    _networkVariable.Value = gameData;
                    break;
                }
            }
        }
    }
}
