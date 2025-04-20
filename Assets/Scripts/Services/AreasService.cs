using UniDi;
using Unity.Netcode;
using UnityEngine;

public class AreasService : NetworkBehaviour
{
    [Inject] AgentAreaDatabase _agentAreaDatabase;
    [Inject] NetworkGameplayService _networkGameplayService;

    public void VisitAgentArea(int areaId, int selectedExchange)
    {
        VisitAgentAreaServerRpc(areaId, selectedExchange);
    }

    [ServerRpc(RequireOwnership = false)]
    void VisitAgentAreaServerRpc(int areaId, int selectedExchange, ServerRpcParams rpcParams = default)
    {
        var agentArea = _agentAreaDatabase.GetItem(areaId);
        var clientId = (ulong)rpcParams.Receive.SenderClientId;


        if (CanVisitArea(clientId, agentArea, selectedExchange))
        {
            var gameData = _networkGameplayService.GameData;
            var playerData = gameData.Players[(int)clientId];
            var exchange = agentArea.Exchanges[selectedExchange];

            bool canPay = true;
            foreach (var cost in exchange.Costs)
            {
                if (!playerData.Items.TryGetValue(cost.Resource.ID, out int inventoryQuantity) || inventoryQuantity < cost.Quantity)
                {
                    canPay = false;
                    break;
                }
            }

            if (canPay)
            {
                foreach (var cost in exchange.Costs)
                {
                    playerData.Items[cost.Resource.ID] -= cost.Quantity;
                }

                foreach (var reward in exchange.Rewards)
                {
                    if (!playerData.Items.ContainsKey(reward.Resource.ID))
                    {
                        playerData.Items.Add(reward.Resource.ID, 0);
                    }
                    playerData.Items[reward.Resource.ID] += reward.Quantity;
                }

                gameData.Players[(int)clientId] = playerData;
                gameData.RandomData = gameData.RandomData + 1; // this enforce network variable change
                _networkGameplayService.UpdateGameData(gameData);
            }
           
            VisitAgentAreaResponseClientRpc(canPay, rpcParams.Receive.SenderClientId);
        }
        else
        {
            VisitAgentAreaResponseClientRpc(false, rpcParams.Receive.SenderClientId);
        }

    }

    [ClientRpc]
    void VisitAgentAreaResponseClientRpc(bool success, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId)
            return;

        Debug.Log($"Request: {success}");
    }

    bool CanVisitArea(ulong clientId, AgentAreaDefinition agentArea, int selectedExchange)
    {
        if (!agentArea)
            return false;

        return true;
    }
}
