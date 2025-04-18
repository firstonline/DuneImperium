using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniDi;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public struct PlayerData : INetworkSerializable
{
    public Dictionary<int, int> Items;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int length = 0;
        int[] keys;
        int[] values;

        if (serializer.IsWriter)
        {
            keys = Items.Keys.ToArray();
            values = Items.Values.ToArray();
            length = Items.Count;
        }
        else
        {
            keys = new int[length];
            values = new int[length];
        }

        serializer.SerializeValue(ref length);
        serializer.SerializeValue(ref keys);
        serializer.SerializeValue(ref values);

        if (serializer.IsReader)
        {
            Items = new Dictionary<int, int>();
            for (int i = 0; i < length; i++)
            {
                Items.Add(keys[i], values[i]);
            }
        }
    }
}

public struct GameData : INetworkSerializable
{
    public List<PlayerData> Players;
    public int RandomData;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {

        serializer.SerializeValue(ref RandomData);

        int length = 0;
        PlayerData[] Array;
        if (serializer.IsWriter)
        {
            Array = Players.ToArray();
            length = Array.Length;
        }
        else
        {
            Array = new PlayerData[length];
        }
        serializer.SerializeValue(ref length);
        serializer.SerializeValue(ref Array);

        if (serializer.IsReader)
        {
            Players = Array.ToList();
        }
    }
}

public class NetworkGameplayService : NetworkBehaviour
{
    [Inject] AgentAreaDatabase _agentAreaDatabase;
    [Inject] ResourcesDatabase _resourcesDatabase;

    [SerializeField] string _connectionScreen;
    [SerializeField] Button _leaveSessionButton;

    NetworkVariable<GameData> _networkVariable = new NetworkVariable<GameData>();
    BehaviorSubject<GameData> _gameData = new BehaviorSubject<GameData>(new GameData());

    public IObservable<GameData> GameData => _gameData;
    public IObservable<PlayerData> ObservePlayerData(int clientId) => _gameData.Select(x =>
    {
        if (x.Players == null || x.Players.Count < clientId)
        {
            var playerData = new PlayerData();
            playerData.Items = new Dictionary<int, int>();
            return playerData;
        }
        else
        {
            return x.Players[clientId];
        }
    });


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (NetworkManager.Singleton == null)
        {
            throw new Exception($"");
        }

        if (NetworkManager.Singleton.IsServer)
        {
            var gameData = new GameData();
    
            gameData.Players = new List<PlayerData> {  };
            for (int i = 0; i < 4; i++)
            {
                var playerData = new PlayerData();
                playerData.Items = new Dictionary<int, int>();
                gameData.Players.Add(playerData);
            }

            _networkVariable.Value = gameData;
            _gameData.OnNext(gameData);
        }

        _networkVariable.OnValueChanged += (oldData, newData) => 
        {
            _gameData.OnNext(newData);
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    public void VisitAgentArea(int clientId, int areaId)
    {
        VisitAgentAreaServerRpc(clientId, areaId);
    }

    [ServerRpc(RequireOwnership = false)]
    void VisitAgentAreaServerRpc(int clientId, int areaId)
    {
        var agentArea = _agentAreaDatabase.GetItem(areaId);
        if (!agentArea)
            return;

        var gameData = _networkVariable.Value;
        var playerData = gameData.Players[clientId];

        var mainExchange = agentArea.Exchanges[0];
        var alternativeExchange = agentArea.Exchanges.Count > 1 ? agentArea.Exchanges[1] : null;


        foreach (var reward in mainExchange.Rewards)
        {
            if (!playerData.Items.ContainsKey(reward.Resource.ID))
            {
                playerData.Items.Add(reward.Resource.ID, 0);
            }
            playerData.Items[reward.Resource.ID] += reward.Quantity;
        }
      
     
        gameData.Players[clientId] = playerData;
        gameData.RandomData = gameData.RandomData + 1; // this enforce network variable change
        _networkVariable.Value = gameData;
    }

    void OnClientDisconnected(ulong clientID)
    {
        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                _leaveSessionButton.onClick.Invoke();
            }
            SceneManager.LoadSceneAsync(_connectionScreen);
        }
    }

}
