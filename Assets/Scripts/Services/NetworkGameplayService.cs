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


public enum ResourceType
{
    Water = 0,
    Solari = 1,
    Spice = 2,
    BeneGesseritInfluence = 3,
    FremenInfluence = 4,
    SpacingGuildInfluence = 5,
    EmperorInfluence = 6,
    MakerHook = 7
}

public struct PlayerData : INetworkSerializable
{
    public static readonly int MAX_TROOPS = 12;
    public static readonly int MAX_INFLUENCE = 6;

    public bool IsTaken;
    public ulong ClientId;

    public bool HasCouncilSeat;
    public bool HaveSwordsman;
    public int AgentsCount;
    public int DeployedAgentsCount;
    public int GarrisonedTroopsCount;
    public int DeployedTroopsCount;
    public int WormsCount;
    public int CombatStrength => WormsCount * 3 + DeployedTroopsCount * 2;

    public Dictionary<ResourceType, int> Resources;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref IsTaken);
        serializer.SerializeValue(ref HasCouncilSeat);
        serializer.SerializeValue(ref HaveSwordsman);
        serializer.SerializeValue(ref AgentsCount);
        serializer.SerializeValue(ref DeployedAgentsCount);
        serializer.SerializeValue(ref GarrisonedTroopsCount);
        serializer.SerializeValue(ref DeployedTroopsCount);
        serializer.SerializeValue(ref WormsCount);

        int length = 0;
        int[] keys;
        int[] values;

        if (serializer.IsWriter)
        {
            keys = Resources.Keys.Select(x => (int)x).ToArray();
            values = Resources.Values.ToArray();
            length = Resources.Count;
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
            Resources = new Dictionary<ResourceType, int>();
            for (int i = 0; i < length; i++)
            {
                Resources.Add((ResourceType)keys[i], values[i]);
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
    public IObservable<PlayerData> ObservePlayerData(int index) => _gameData.Select(x =>
    {
        if (x.Players == null || x.Players.Count < index)
        {
            var playerData = new PlayerData();
            playerData.Resources = GenerateInventory();
            playerData.AgentsCount = 2;
            playerData.Resources[ResourceType.Water]++;
            return playerData;
        }
        else
        {
            return x.Players[index];
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
                playerData.Resources = GenerateInventory();
                playerData.AgentsCount = 2;
                playerData.Resources[ResourceType.Water]++;
                if (i == 0)
                {
                    playerData.IsTaken = true;
                }

                gameData.Players.Add(playerData);
            }

            _networkVariable.Value = gameData;
            _gameData.OnNext(gameData);
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        else
        {
            _gameData.OnNext(_networkVariable.Value);
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
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    public void UpdateGameData(GameData gameData)
    {
        if (NetworkManager.Singleton != null)
        {
            _networkVariable.Value = gameData;
        }
    }

    Dictionary<ResourceType, int> GenerateInventory()
    {
        var inventory = new Dictionary<ResourceType, int>();
        var itemTypes = Enum.GetValues(typeof(ResourceType));
        foreach (var itemType in itemTypes)
        {
            inventory.Add((ResourceType)itemType, 0);
        }
        return inventory;
    }

    void OnClientConnected(ulong clientID)
    {
        Debug.Log($"Client Connected {clientID}");
        var gameData = _networkVariable.Value;

        for (int i = 0; i < gameData.Players.Count; i++)
        {
            var player = gameData.Players[i];
            if (!player.IsTaken)
            {
                player.IsTaken = true;
                player.ClientId = clientID;
                gameData.Players[i] = player;
                gameData.RandomData++;
                _networkVariable.Value = gameData;
                Debug.Log($"Assigning client {player.ClientId} to {i}");
                break;
            }
        }
    }

    void OnClientDisconnected(ulong clientID)
    {
        Debug.Log($"Client Disconnected {clientID}");

        if (clientID == NetworkManager.Singleton.LocalClientId)
        {
            if (!NetworkManager.Singleton.IsHost)
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
                    gameData.RandomData++;
                    _networkVariable.Value = gameData;

                    Debug.Log($"Unassigning client {player.ClientId} from {i}");
                    break;
                }
            }
        }
    }

    
}
