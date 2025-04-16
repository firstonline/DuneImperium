using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct PlayerData : INetworkSerializable
{
    public int RandomData;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref RandomData);
    }
}

public class Player : NetworkBehaviour
{
    [SerializeField] string _connectionScreen;
    [SerializeField] Button _leaveSessionButton;

    NetworkVariable<PlayerData> _networkVariable = new NetworkVariable<PlayerData>();


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (NetworkManager.Singleton == null)
        {
            // Can't listen to something that doesn't exist >:(
            throw new Exception($"");
        }
        _networkVariable.OnValueChanged += (oldData, newData) => { Debug.Log($"NewData {newData.RandomData}"); };

        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    public void Testing()
    {
        ServerRpc();
    }

    private void OnClientDisconnected(ulong clientID)
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

    [ServerRpc(RequireOwnership = false)]
    void ServerRpc()
    {
        var updatedValue = _networkVariable.Value;
        updatedValue.RandomData++;

        _networkVariable.Value = updatedValue;
        UpdateClientRpc();
    }

    [ClientRpc]
    void UpdateClientRpc()
    {
    }

}
