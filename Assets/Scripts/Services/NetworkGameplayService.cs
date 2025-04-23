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

public enum ItemType
{
    Water = 0,
    Solari = 1,
    Spice = 2,
}

public struct PlayerData : INetworkSerializable
{
    public Dictionary<ItemType, int> Inventory;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int length = 0;
        int[] keys;
        int[] values;

        if (serializer.IsWriter)
        {
            keys = Inventory.Keys.Select(x => (int)x).ToArray();
            values = Inventory.Values.ToArray();
            length = Inventory.Count;
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
            Inventory = new Dictionary<ItemType, int>();
            for (int i = 0; i < length; i++)
            {
                Inventory.Add((ItemType)keys[i], values[i]);
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
    [Inject] AreasService _areasService;

    [SerializeField] string _connectionScreen;
    [SerializeField] Button _leaveSessionButton;

    NetworkVariable<GameData> _networkVariable = new NetworkVariable<GameData>();
    BehaviorSubject<GameData> _gameData = new BehaviorSubject<GameData>(new GameData());

    public GameData GameData => _gameData.Value;
    public IObservable<PlayerData> ObservePlayerData(int clientId) => _gameData.Select(x =>
    {
        if (x.Players == null || x.Players.Count < clientId)
        {
            var playerData = new PlayerData();
            playerData.Inventory = GenerateInventory();
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
                playerData.Inventory = GenerateInventory();
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

    public void UpdateGameData(GameData gameData)
    {
        if (NetworkManager.Singleton != null)
        {
            _networkVariable.Value = gameData;
        }
    }

    Dictionary<ItemType, int> GenerateInventory()
    {
        var inventory = new Dictionary<ItemType, int>();
        var itemTypes = Enum.GetValues(typeof(ItemType));
        foreach (var itemType in itemTypes)
        {
            inventory.Add((ItemType)itemType, 0);
        }
        return inventory;
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
